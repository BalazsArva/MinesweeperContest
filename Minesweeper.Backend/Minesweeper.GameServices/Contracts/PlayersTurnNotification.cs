using MediatR;

namespace Minesweeper.GameServices.Contracts
{
    public class PlayersTurnNotification : INotification
    {
        // TODO: Create handler
        public PlayersTurnNotification(string gameId, string playerId)
        {
            GameId = gameId;
            PlayerId = playerId;
        }

        public string GameId { get; }

        public string PlayerId { get; }
    }
}