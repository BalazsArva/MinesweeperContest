namespace Minesweeper.GameServices.Contracts.Commands
{
    public class MakeMoveCommand
    {
        public string GameId { get; set; }

        public string PlayerId { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }
    }
}