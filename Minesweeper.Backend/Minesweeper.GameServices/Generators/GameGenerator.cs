using System;
using System.Linq;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Providers;

namespace Minesweeper.GameServices.Generators
{
    public class GameGenerator : IGameGenerator
    {
        private readonly IRandomPlayerSelector _randomPlayerSelector;
        private readonly IRandomNumberProvider _randomNumberProvider;

        public GameGenerator(IRandomPlayerSelector randomPlayerSelector, IRandomNumberProvider randomNumberProvider)
        {
            _randomPlayerSelector = randomPlayerSelector ?? throw new ArgumentNullException(nameof(randomPlayerSelector));
            _randomNumberProvider = randomNumberProvider ?? throw new ArgumentNullException(nameof(randomNumberProvider));
        }

        public Game GenerateGame(int tableRows, int tableColumns, int mineCount)
        {
            var starterPlayer = _randomPlayerSelector.SelectRandomPlayer();
            var visibleTable = new VisibleFieldType[tableRows][];

            for (var row = 0; row < tableRows; ++row)
            {
                visibleTable[row] = new VisibleFieldType[tableColumns];

                for (var col = 0; col < tableColumns; ++col)
                {
                    visibleTable[row][col] = VisibleFieldType.Unknown;
                }
            }

            return new Game
            {
                BaseTable = GenerateBaseTable(tableRows, tableColumns, mineCount),
                VisibleTable = visibleTable,
                NextPlayer = starterPlayer,
                Status = GameStatus.NotStarted,
                Rows = tableRows,
                Columns = tableColumns,
                Mines = mineCount,
                Player1 = new Player(),
                Player2 = new Player()
            };
        }

        private FieldTypes[][] GenerateBaseTable(int rows, int columns, int mineCount)
        {
            var gameTable = new FieldTypes[rows][];

            var minedFields = Enumerable
                .Range(0, rows)
                .Select(row => Enumerable.Range(0, columns).Select(col => new { Row = row, Column = col }))
                .SelectMany(pairs => pairs)
                .Select(pair => new
                {
                    pair.Row,
                    pair.Column,
                    Order = _randomNumberProvider.GetRandomNumber()
                })
                .OrderBy(pairWithOrder => pairWithOrder.Order)
                .Take(mineCount)
                .ToList();

            for (var row = 0; row < rows; ++row)
            {
                gameTable[row] = new FieldTypes[columns];

                for (var col = 0; col < columns; ++col)
                {
                    gameTable[row][col] = FieldTypes.Empty;
                }
            }

            foreach (var minedField in minedFields)
            {
                gameTable[minedField.Row][minedField.Column] = FieldTypes.Mined;
            }

            return gameTable;
        }
    }
}