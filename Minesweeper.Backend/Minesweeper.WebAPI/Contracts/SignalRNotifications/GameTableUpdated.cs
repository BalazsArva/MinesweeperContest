using System.Collections.Generic;
using System.Linq;

namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class GameTableUpdated
    {
        public GameTableUpdated(IEnumerable<GameServices.Contracts.FieldUpdate> fieldUpdates)
        {
            FieldUpdates = fieldUpdates.Select(u => new FieldUpdate(u)).ToList();
        }

        public IEnumerable<FieldUpdate> FieldUpdates { get; }
    }
}