using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class RemainingMinesChangedNotification : INotification
    {
        public string GameId { get; set; }

        public int RemainingMineCount { get; set; }
    }
}