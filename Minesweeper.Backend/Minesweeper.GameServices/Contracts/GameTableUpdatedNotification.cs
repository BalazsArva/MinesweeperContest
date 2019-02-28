using MediatR;

namespace Minesweeper.GameServices.Contracts
{
    public class GameTableUpdatedNotification : INotification
    {
        public GameTableUpdatedNotification(string gameId, VisibleFieldType[,] table)
        {
            GameId = gameId;
            Table = table;
        }

        public string GameId { get; }

        public VisibleFieldType[,] Table { get; }
    }
}