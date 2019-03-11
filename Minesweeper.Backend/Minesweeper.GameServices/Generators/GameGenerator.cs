using System;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.Generators
{
    public class GameGenerator : IGameGenerator
    {
        private readonly IGameTableGenerator _gameTableGenerator;
        private readonly IRandomPlayerSelector _randomPlayerSelector;

        public GameGenerator(IGameTableGenerator gameTableGenerator, IRandomPlayerSelector randomPlayerSelector)
        {
            _gameTableGenerator = gameTableGenerator ?? throw new ArgumentNullException(nameof(gameTableGenerator));
            _randomPlayerSelector = randomPlayerSelector ?? throw new ArgumentNullException(nameof(randomPlayerSelector));
        }

        public Game GenerateGame(int tableRows, int tableColumns, int mineCount)
        {
            var gameTable = _gameTableGenerator.GenerateGameTable(tableRows, tableColumns, mineCount);
            var starterPlayer = _randomPlayerSelector.SelectRandomPlayer();

            return new Game
            {
                GameTable = gameTable,
                Player1Marks = new MarkTypes[tableRows, tableColumns],
                Player2Marks = new MarkTypes[tableRows, tableColumns],
                StarterPlayer = starterPlayer,
                Status = GameStatus.NotStarted
            };
        }
    }
}