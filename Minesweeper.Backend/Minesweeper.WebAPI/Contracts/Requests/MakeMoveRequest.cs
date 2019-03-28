using Newtonsoft.Json;

namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class MakeMoveRequest
    {
        [JsonConstructor]
        public MakeMoveRequest(int row, int column)
        {
            Row = row;
            Column = column;
        }

        public int Row { get; }

        public int Column { get; }
    }
}