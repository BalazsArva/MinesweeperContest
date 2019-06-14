namespace Minesweeper.WebAPI.Contracts.Responses.Lobby
{
    public class PlayersGame
    {
        public string GameId { get; set; }

        public string OtherPlayerId { get; set; }

        public string OtherPlayerDisplayName { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public int Mines { get; set; }
    }
}