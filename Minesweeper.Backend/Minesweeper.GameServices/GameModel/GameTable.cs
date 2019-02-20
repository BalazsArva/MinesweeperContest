using System;

namespace Minesweeper.GameServices.GameModel
{
    public class GameTable
    {
        public const int MinRows = 10;
        public const int MaxRows = 100;

        public const int MinColumns = 10;
        public const int MaxColumns = 100;

        public GameTable(int rows, int columns)
        {
            if (rows < MinRows)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), $"Parameter '{nameof(rows)}' cannot be less than {MinRows}.");
            }

            if (rows > MaxRows)
            {
                throw new ArgumentOutOfRangeException(nameof(rows), $"Parameter '{nameof(rows)}' cannot be greater than {MaxRows}.");
            }

            if (rows < MinColumns)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), $"Parameter '{nameof(columns)}' cannot be less than {MinColumns}.");
            }

            if (rows > MaxColumns)
            {
                throw new ArgumentOutOfRangeException(nameof(columns), $"Parameter '{nameof(columns)}' cannot be greater than {MaxColumns}.");
            }

            Rows = rows;
            Columns = columns;

            FieldMatrix = new FieldTypes[rows, columns];
        }

        public int Rows { get; }

        public int Columns { get; }

        public FieldTypes[,] FieldMatrix { get; }
    }
}