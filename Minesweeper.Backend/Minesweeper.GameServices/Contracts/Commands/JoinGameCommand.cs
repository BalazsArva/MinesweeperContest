namespace Minesweeper.GameServices.Contracts.Commands
{
    public class JoinGameCommand
    {
        public string GameId { get; set; }

        public string PlayerId { get; set; }

        public string PlayerDisplayName { get; set; }
    }
}