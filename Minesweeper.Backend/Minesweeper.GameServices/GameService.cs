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
    }
}