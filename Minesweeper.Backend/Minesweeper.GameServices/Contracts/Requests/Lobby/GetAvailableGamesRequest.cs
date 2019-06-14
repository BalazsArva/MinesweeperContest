namespace Minesweeper.GameServices.Contracts.Requests.Lobby
{
    public class GetAvailableGamesRequest
    {
        public string UserId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }
    }
}