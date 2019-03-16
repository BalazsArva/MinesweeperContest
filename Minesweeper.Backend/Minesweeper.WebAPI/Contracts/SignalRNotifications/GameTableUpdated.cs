using System.Collections.Generic;
using Minesweeper.GameServices.Contracts;

namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class GameTableUpdated
    {
        public GameTableUpdated(IEnumerable<GameTableUpdatedNotification.FieldUpdate> fieldUpdates)
        {
            // TODO: Introduce own contract
            FieldUpdates = fieldUpdates;
        }

        public IEnumerable<GameTableUpdatedNotification.FieldUpdate> FieldUpdates { get; }
    }
}