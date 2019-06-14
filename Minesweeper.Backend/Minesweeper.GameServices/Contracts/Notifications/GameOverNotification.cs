using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class GameOverNotification : INotification
    {
        public string GameId { get; set; }

        public string WinnerPlayerId { get; set; }
    }
}