using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Generators;
using Minesweeper.GameServices.Providers;
using Raven.Client.Documents;

namespace Minesweeper.GameServices
{
    public class GameService : IGameService
    {
        private readonly IMediator _mediator;
        private readonly IDocumentStore _documentStore;
        private readonly IGameGenerator _gameGenerator;
        private readonly IGameDriver _gameDriver;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IGuidProvider _guidProvider;
        private readonly IGameTableVisibilityComputer _gameTableVisibilityComputer;

        public GameService(IMediator mediator, IDocumentStore documentStore, IGameGenerator gameGenerator, IGameDriver gameDriver, IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider, IGameTableVisibilityComputer gameTableVisibilityComputer)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _gameGenerator = gameGenerator ?? throw new ArgumentNullException(nameof(gameGenerator));
            _gameDriver = gameDriver ?? throw new ArgumentNullException(nameof(gameDriver));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
            _gameTableVisibilityComputer = gameTableVisibilityComputer ?? throw new ArgumentNullException(nameof(gameTableVisibilityComputer));
        }

        public async Task<string> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, string invitedPlayerId, int tableRows, int tableColumns, int mineCount, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = _gameGenerator.GenerateGame(tableRows, tableColumns, mineCount);

                game.Id = _documentStore.GetPrefixedDocumentId<Game>(_guidProvider.GenerateGuidString());
                game.InvitedPlayerId = string.IsNullOrWhiteSpace(invitedPlayerId) ? null : invitedPlayerId;
                game.Player1 = new Player
                {
                    PlayerId = hostPlayerId,
                    DisplayName = hostPlayerDisplayName,
                    Points = 0
                };

                await session.StoreAsync(game, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return game.Id;
            }
        }

        public async Task JoinGameAsync(string gameId, string player2Id, string player2DisplayName, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);
                var changeVector = session.Advanced.GetChangeVectorFor(game);

                if (game == null)
                {
                    throw new GameNotFoundException();
                }

                if ((game.InvitedPlayerId != null && game.InvitedPlayerId != player2Id) || game.Player2 != null || game.Player1.PlayerId == player2Id)
                {
                    throw new ActionNotAllowedException("You are not allowed to join the requested game.");
                }

                game.UtcDateTimeStarted = _dateTimeProvider.GetUtcDateTime();
                game.Player2 = new Player
                {
                    PlayerId = player2Id,
                    DisplayName = player2DisplayName,
                    Points = 0
                };

                // TODO: Investigate what exception is thrown when a concurrent update occurs (because of the changevector) and rethrow an appropriate custom exception
                await session.StoreAsync(game, changeVector, game.Id, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<MoveResult> MakeMoveAsync(string gameId, string playerId, int row, int column, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Concurrency protection
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                var movementResult = _gameDriver.MakeMove(game, playerId, row, column);
                if (movementResult == MoveResultType.Success || movementResult == MoveResultType.GameOver)
                {
                    await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                    await PublishTableUpdatedAsync(gameId, cancellationToken);
                }

                return new MoveResult { MoveResultType = movementResult };
            }
        }

        public async Task MarkFieldAsync(string gameId, string playerId, int row, int column, MarkType markType, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                if (game.Player1.PlayerId != playerId && game.Player2.PlayerId != playerId)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                var isPlayer1 = game.Player1.PlayerId == playerId;
                var marks = game.Player1.PlayerId == playerId ? game.Player1Marks : game.Player2Marks;

                // TODO: Index overflow and underflow check here and everywhere else
                marks[row, column] = MapContractMarkTypeToEntityMarkType(markType);

                if (isPlayer1)
                {
                    // TODO: This thros an exception. Fix it.
                    session.Advanced.Patch<Game, MarkTypes[,]>(game.Id, g => g.Player1Marks, marks);
                }
                else
                {
                    session.Advanced.Patch<Game, MarkTypes[,]>(game.Id, g => g.Player2Marks, marks);
                }

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<MarkType[,]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                var isPlayer1 = game.Player1.PlayerId == playerId;
                var isPlayer2 = game.Player2.PlayerId == playerId;

                if (!isPlayer1 && !isPlayer2)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                var playerMarks = isPlayer1 ? game.Player1Marks : game.Player2Marks;
                var result = new MarkType[playerMarks.GetLength(0), playerMarks.GetLength(1)];

                for (var row = 0; row < playerMarks.GetLength(0); ++row)
                {
                    for (var col = 0; col < playerMarks.GetLength(1); ++col)
                    {
                        result[row, col] = MapEntityMarkTypeToContractMarkType(playerMarks[row, col]);
                    }
                }

                return result;
            }
        }

        public async Task<VisibleFieldType[,]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Validate user id
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                return _gameTableVisibilityComputer.GetVisibleGameTableAsync(game);
            }
        }

        private async Task PublishTableUpdatedAsync(string gameId, CancellationToken cancellationToken)
        {
            // TODO: This is only an initial skeleton. Refactor so that only the changed fields are included.
            var table = await GetVisibleGameTableAsync(gameId, cancellationToken);

            await _mediator.Publish(new GameTableUpdatedNotification(gameId, table), cancellationToken);
        }

        private MarkType MapEntityMarkTypeToContractMarkType(MarkTypes markType)
        {
            return markType == MarkTypes.Empty
                ? MarkType.Empty
                : markType == MarkTypes.None
                    ? MarkType.None
                    : MarkType.Unknown;
        }

        private MarkTypes MapContractMarkTypeToEntityMarkType(MarkType markType)
        {
            return markType == MarkType.Empty
                ? MarkTypes.Empty
                : markType == MarkType.None
                    ? MarkTypes.None
                    : MarkTypes.Unknown;
        }
    }
}