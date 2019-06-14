namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class NewGameRequest
    {
        public int TableRows { get; set; }

        public int TableColumns { get; set; }

        public int MineCount { get; set; }

        public string InvitedPlayerId { get; set; }
    }
}