namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class JoinGameRequest
    {
        public string PlayerId { get; set; }

        public string EntryToken { get; set; }

        public string DisplayName { get; set; }
    }
}