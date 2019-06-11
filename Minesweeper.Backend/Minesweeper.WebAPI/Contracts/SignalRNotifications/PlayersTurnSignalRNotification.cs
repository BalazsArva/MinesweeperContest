namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class PlayersTurnSignalRNotification
    {
        public PlayersTurnSignalRNotification(string playerId)
        {
            PlayerId = playerId;
        }

        public string PlayerId { get; }
    }
}