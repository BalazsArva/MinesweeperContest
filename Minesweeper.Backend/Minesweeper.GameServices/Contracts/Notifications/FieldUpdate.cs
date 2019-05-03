using Minesweeper.GameServices.Contracts.Responses;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class FieldUpdate
    {
        public FieldUpdate(int row, int column, VisibleFieldType fieldType)
        {
            Row = row;
            Column = column;
            FieldType = fieldType;
        }

        public int Row { get; }

        public int Column { get; }

        public VisibleFieldType FieldType { get; }
    }
}