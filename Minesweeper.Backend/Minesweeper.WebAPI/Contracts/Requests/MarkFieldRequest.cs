using Minesweeper.GameServices.Contracts;

namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class MarkFieldRequest
    {
        public int Row { get; set; }

        public int Column { get; set; }

        public MarkType MarkType { get; set; }
    }
}