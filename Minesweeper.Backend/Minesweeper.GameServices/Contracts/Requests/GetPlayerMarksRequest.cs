namespace Minesweeper.GameServices.Contracts.Requests
{
    public class GetPlayerMarksRequest
    {
        public string GameId { get; set; }

        public string PlayerId { get; set; }
    }
}