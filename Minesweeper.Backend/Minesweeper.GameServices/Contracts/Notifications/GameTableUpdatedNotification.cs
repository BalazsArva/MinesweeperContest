using System.Collections.Generic;
using MediatR;

namespace Minesweeper.GameServices.Contracts.Notifications
{
    public class GameTableUpdatedNotification : INotification
    {
        public GameTableUpdatedNotification(string gameId, IEnumerable<FieldUpdate> fieldUpdates)
        {
            GameId = gameId;
            FieldUpdates = fieldUpdates;
        }

        public string GameId { get; }

        public IEnumerable<FieldUpdate> FieldUpdates { get; }
    }
}