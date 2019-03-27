using System.Collections.Generic;
using MediatR;

namespace Minesweeper.GameServices.Contracts
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

    public class FieldUpdate
    {
        public FieldUpdate(int row, int column, VisibleFieldType fieldType)
        {
            Row = row;
            Column = column;
            FieldType = fieldType;
        }

        public int Row { get; }

        public int Column { get; }

        public VisibleFieldType FieldType { get; }
    }
}