using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class GameOverNotification : INotification
    {
        public GameOverNotification(string gameId, string winnerPlayerId)
        {
            GameId = gameId;
            WinnerPlayerId = winnerPlayerId;
        }

        public string GameId { get; }

        public string WinnerPlayerId { get; }
    }
}