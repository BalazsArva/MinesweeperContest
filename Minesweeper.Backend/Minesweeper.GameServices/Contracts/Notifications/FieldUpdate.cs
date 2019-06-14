using Minesweeper.GameServices.Contracts.Responses.Game;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class FieldUpdate
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public VisibleFieldType FieldType { get; set; }
    }
}