using System;

namespace Minesweeper.GameServices.GameModel
{
    public class GameTable
    {
        public const int MinRows = 10;
        public const int MaxRows = 100;

        public const int MinColumns = 10;
        public const int MaxColumns = 100;

        protected GameTable()
        {
        }

        public GameTable(int rows, int columns, int mines)
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

            // TODO: Review whether this is appropriate here, it is a difficulty concern, not directly related to the table as a data structure. Maybe should implemen a difficulty prop on the game itself.
            Mines = mines;

            FieldMatrix = new FieldTypes[rows, columns];
        }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public int Mines { get; set; }

        public FieldTypes[,] FieldMatrix { get; set; }
    }
}