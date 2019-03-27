using System;

namespace Minesweeper.WebAPI.Contracts.SignalRNotifications
{
    public class FieldUpdate
    {
        public FieldUpdate(GameServices.Contracts.FieldUpdate serviceContract)
        {
            Row = serviceContract.Row;
            Column = serviceContract.Column;
            FieldType = Enum.Parse<VisibleFieldType>(serviceContract.FieldType.ToString());
        }

        public int Row { get; }

        public int Column { get; }

        public VisibleFieldType FieldType { get; }
    }
}