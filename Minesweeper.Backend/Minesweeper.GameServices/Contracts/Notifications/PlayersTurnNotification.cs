using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class PlayersTurnNotification : INotification
    {
        public string GameId { get; set; }

        public string PlayerId { get; set; }
    }
}