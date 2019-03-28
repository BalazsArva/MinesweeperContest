using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Cloners;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Converters;
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

        public GameService(IMediator mediator, IDocumentStore documentStore, IGameGenerator gameGenerator, IGameDriver gameDriver, IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _gameGenerator = gameGenerator ?? throw new ArgumentNullException(nameof(gameGenerator));
            _gameDriver = gameDriver ?? throw new ArgumentNullException(nameof(gameDriver));
            _dateTimeProvider = dateTimeProvider ?? throw new ArgumentNullException(nameof(dateTimeProvider));
            _guidProvider = guidProvider ?? throw new ArgumentNullException(nameof(guidProvider));
        }

        public async Task<string> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, string invitedPlayerId, int tableRows, int tableColumns, int mineCount, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = _gameGenerator.GenerateGame(tableRows, tableColumns, mineCount);

                // TODO: Validate that the invited player's Id is not the same as the host's Id.
                game.Id = _documentStore.GetPrefixedDocumentId<Game>(_guidProvider.GenerateGuidString());
                game.InvitedPlayerId = string.IsNullOrWhiteSpace(invitedPlayerId) ? null : invitedPlayerId;
                game.Player1.PlayerId = hostPlayerId;
                game.Player1.DisplayName = hostPlayerDisplayName;

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

                if ((game.InvitedPlayerId != null && game.InvitedPlayerId != player2Id) || game.Player2.PlayerId != null || game.Player1.PlayerId == player2Id)
                {
                    throw new ActionNotAllowedException("You are not allowed to join the requested game.");
                }

                game.UtcDateTimeStarted = _dateTimeProvider.GetUtcDateTime();
                game.Player2.PlayerId = player2Id;
                game.Player2.DisplayName = player2DisplayName;

                // TODO: Investigate what exception is thrown when a concurrent update occurs (because of the changevector) and rethrow an appropriate custom exception
                await session.StoreAsync(game, changeVector, game.Id, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task MarkFieldAsync(string gameId, string playerId, int row, int column, Contracts.MarkTypes markType, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                if (game.Player1.PlayerId != playerId && game.Player2.PlayerId != playerId)
                {
                    throw new ActionNotAllowedException("You are not involved in that game.");
                }

                // TODO: Index overflow and underflow check here and everywhere else
                var isPlayer1 = game.Player1.PlayerId == playerId;

                var newMark = MarkTypeConverter.FromContract(markType);

                if (isPlayer1)
                {
                    session.Advanced.Patch<Game, GameModel.MarkTypes>(game.Id, g => g.Player1Marks[row][column], newMark);
                }
                else
                {
                    session.Advanced.Patch<Game, GameModel.MarkTypes>(game.Id, g => g.Player2Marks[row][column], newMark);
                }

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<Contracts.MarkTypes[][]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken)
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

                return EnumArrayCloner.CloneAndMap(playerMarks, MarkTypeConverter.ToContract);
            }
        }

        public async Task<Contracts.VisibleFieldType[][]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Validate user id
                var game = await session.LoadGameAsync(gameId, cancellationToken).ConfigureAwait(false);

                return EnumArrayCloner.CloneAndMap(game.VisibleTable, FieldTypeConverter.ToContract);
            }
        }
    }
}