using System;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.DataStructures
{
    public class FieldLookup
    {
        private readonly bool[,] registeredFields;

        public FieldLookup(int rows, int columns)
        {
            registeredFields = new bool[rows, columns];
        }

        public static FieldLookup FromGame(Game game)
        {
            if (game == null)
            {
                throw new ArgumentNullException(nameof(game));
            }

            var result = new FieldLookup(game.GameTable.Rows, game.GameTable.Columns);

            foreach (var move in game.Moves)
            {
                result.RegisterField(move.Row, move.Column);
            }

            return result;
        }

        public void RegisterField(int row, int column)
        {
            registeredFields[row, column] = true;
        }

        public bool IsFieldRegistered(int row, int column)
        {
            return registeredFields[row, column];
        }
    }
}