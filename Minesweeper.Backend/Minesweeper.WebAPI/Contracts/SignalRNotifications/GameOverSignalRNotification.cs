namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class GameOverSignalRNotification
    {
        public GameOverSignalRNotification(string winnerPlayerId)
        {
            WinnerPlayerId = winnerPlayerId;
        }

        public string WinnerPlayerId { get; }
    }
}