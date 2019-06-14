using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class PlayerPointsChangedNotification : INotification
    {
        public string GameId { get; set; }

        public string PlayerId { get; set; }

        public int Points { get; set; }
    }
}