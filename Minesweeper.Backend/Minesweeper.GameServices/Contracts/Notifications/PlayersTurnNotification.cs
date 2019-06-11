using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class PlayersTurnNotification : INotification
    {
        public PlayersTurnNotification(string gameId, string playerId)
        {
            GameId = gameId;
            PlayerId = playerId;
        }

        public string GameId { get; }

        public string PlayerId { get; }
    }
}