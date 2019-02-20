using System;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Generators;

namespace Minesweeper.GameServices
{
    public class GameService : IGameService
    {
        private readonly IGameGenerator _gameGenerator;

        public GameService(IGameGenerator gameGenerator)
        {
            _gameGenerator = gameGenerator ?? throw new ArgumentNullException(nameof(gameGenerator));
        }

        public async Task<NewGameInfo> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, int tableRows, int tableColumns, int mineCount, CancellationToken cancellationToken)
        {
            var game = _gameGenerator.GenerateGame(tableRows, tableColumns, mineCount);

            game.Player1 = new Player(hostPlayerId, hostPlayerDisplayName);

            return new NewGameInfo(game.Id, game.EntryToken);
        }

        public async Task JoinGameAsync(string gameId, string player2Id, string playerDisplayName, string entryToken, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<MoveResult> MakeMoveAsync(string gameId, string playerId, int row, int column, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}