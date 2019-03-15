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

            var player1Marks = new MarkTypes[tableRows][];
            var player2Marks = new MarkTypes[tableRows][];

            for (var row = 0; row < tableRows; ++row)
            {
                player1Marks[row] = new MarkTypes[tableColumns];
                player2Marks[row] = new MarkTypes[tableColumns];

                for (var col = 0; col < tableColumns; ++col)
                {
                    player1Marks[row][col] = MarkTypes.None;
                    player2Marks[row][col] = MarkTypes.None;
                }
            }

            return new Game
            {
                GameTable = gameTable,
                Player1Marks = player1Marks,
                Player2Marks = player2Marks,
                StarterPlayer = starterPlayer,
                Status = GameStatus.NotStarted
            };
        }
    }
}