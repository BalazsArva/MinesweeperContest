using Minesweeper.GameServices.Contracts.Notifications;

namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class RemainingMinesChangedSignalRNotification
    {
        public RemainingMinesChangedSignalRNotification(RemainingMinesChangedNotification serviceNotification)
        {
            RemainingMineCount = serviceNotification.RemainingMineCount;
        }

        public int RemainingMineCount { get; }
    }
}