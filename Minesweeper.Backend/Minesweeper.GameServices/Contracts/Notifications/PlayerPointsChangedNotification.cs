using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class PlayerPointsChangedNotification : INotification
    {
        public PlayerPointsChangedNotification(string gameId, string playerId, int points)
        {
            GameId = gameId;
            PlayerId = playerId;
            Points = points;
        }

        public string GameId { get; }

        public string PlayerId { get; }

        public int Points { get; }
    }
}