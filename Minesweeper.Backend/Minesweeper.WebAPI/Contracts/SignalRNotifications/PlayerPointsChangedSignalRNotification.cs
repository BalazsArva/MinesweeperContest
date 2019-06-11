namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class PlayerPointsChangedSignalRNotification
    {
        public PlayerPointsChangedSignalRNotification(string playerId, int points)
        {
            PlayerId = playerId;
            Points = points;
        }

        public string PlayerId { get; }

        public int Points { get; }
    }
}