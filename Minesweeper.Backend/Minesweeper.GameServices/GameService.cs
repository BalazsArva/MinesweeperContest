using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Generators;
using Raven.Client.Documents;

namespace Minesweeper.GameServices
{
    public class GameService : IGameService
    {
        private readonly IDocumentStore _documentStore;
        private readonly IGameGenerator _gameGenerator;

        public GameService(IDocumentStore documentStore, IGameGenerator gameGenerator)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
            _gameGenerator = gameGenerator ?? throw new ArgumentNullException(nameof(gameGenerator));
        }

        public async Task<NewGameInfo> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, int tableRows, int tableColumns, int mineCount, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var game = _gameGenerator.GenerateGame(tableRows, tableColumns, mineCount);

                game.Player1 = new Player(hostPlayerId, hostPlayerDisplayName);

                await session.StoreAsync(game, cancellationToken).ConfigureAwait(false);
                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                return new NewGameInfo(game.Id, game.EntryToken);
            }
        }

        public async Task JoinGameAsync(string gameId, string player2Id, string playerDisplayName, string entryToken, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                // TODO: Create util method for document id prefix generation
                var game = await session.LoadAsync<Game>("Games/" + gameId, cancellationToken).ConfigureAwait(false);

                // TODO: Validate: Player2 is unset, entry token is valid, game not finished, etc.
                game.Player2 = new Player(player2Id, playerDisplayName);

                await session.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        public async Task<MoveResult> MakeMoveAsync(string gameId, string playerId, int row, int column, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}