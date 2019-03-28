using MediatR;

namespace Minesweeper.GameServices.Contracts
{
    public class RemainingMinesChangedNotification : INotification
    {
        public RemainingMinesChangedNotification(string gameId, int remainingMineCount)
        {
            GameId = gameId;
            RemainingMineCount = remainingMineCount;
        }

        public string GameId { get; }

        public int RemainingMineCount { get; }
    }
}