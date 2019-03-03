using Minesweeper.GameServices.Contracts;

namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class GameTableUpdated
    {
        public GameTableUpdated(VisibleFieldType[,] table)
        {
            Table = table;
        }

        public VisibleFieldType[,] Table { get; }
    }
}