namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class MakeMoveRequest
    {
        public int Row { get; set; }

        public int Column { get; set; }
    }
}