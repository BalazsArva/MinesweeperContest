namespace Minesweeper.GameServices.Contracts.Requests.Game
{
    public class GetPlayerMarksRequest
    {
        public string GameId { get; set; }

        public string PlayerId { get; set; }
    }
}