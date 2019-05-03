using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class GameTableUpdatedSignalRNotification
    {
        public GameTableUpdatedSignalRNotification(IEnumerable<GameServices.Contracts.Notifications.FieldUpdate> fieldUpdates)
        {
            FieldUpdates = fieldUpdates.Select(u => new FieldUpdate(u)).ToList();
        }

        public IEnumerable<FieldUpdate> FieldUpdates { get; }
    }
}