namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class NewGameRequest
    {
        public string PlayerId { get; set; }

        public string DisplayName { get; set; }

        public int TableRows { get; set; }

        public int TableColumns { get; set; }

        public int MineCount { get; set; }
    }
}