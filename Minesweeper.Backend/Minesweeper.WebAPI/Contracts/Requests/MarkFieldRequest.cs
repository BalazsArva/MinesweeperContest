using Minesweeper.GameServices.Contracts;
using Newtonsoft.Json;

namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class MarkFieldRequest
    {
        [JsonConstructor]
        public MarkFieldRequest(int row, int column, MarkTypes markType)

        {
            Row = row;
            Column = column;
            MarkType = markType;
        }

        public int Row { get; }

        public int Column { get; }

        public MarkTypes MarkType { get; }
    }
}