namespace Minesweeper.WebAPI.Contracts.Responses.Lobby
{
    public class AvailableGame
    {
        public string GameId { get; set; }

        public string HostPlayerId { get; set; }

        public string HostPlayerDisplayName { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public int Mines { get; set; }
    }
}