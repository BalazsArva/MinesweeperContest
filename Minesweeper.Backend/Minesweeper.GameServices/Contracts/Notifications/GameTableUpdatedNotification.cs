using System.Collections.Generic;
using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class GameTableUpdatedNotification : INotification
    {
        public string GameId { get; set; }

        public IEnumerable<FieldUpdate> FieldUpdates { get; set; }
    }
}