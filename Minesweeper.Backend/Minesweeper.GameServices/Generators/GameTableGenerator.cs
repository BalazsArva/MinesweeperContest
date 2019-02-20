using System;
using System.Linq;
using Minesweeper.GameServices.GameModel;
using Minesweeper.GameServices.Providers;

namespace Minesweeper.GameServices.Generators
{
    public class GameTableGenerator : IGameTableGenerator
    {
        private readonly IRandomNumberProvider _randomNumberProvider;

        public GameTableGenerator(IRandomNumberProvider randomNumberProvider)
        {
            _randomNumberProvider = randomNumberProvider ?? throw new ArgumentNullException(nameof(randomNumberProvider));
        }

        public GameTable GenerateGameTable(int rows, int columns, int mineCount)
        {
            var gameTable = new GameTable(rows, columns);

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

            foreach (var minedField in minedFields)
            {
                gameTable[minedField.Row, minedField.Column] = FieldTypes.Mined;
            }

            return gameTable;
        }
    }
}