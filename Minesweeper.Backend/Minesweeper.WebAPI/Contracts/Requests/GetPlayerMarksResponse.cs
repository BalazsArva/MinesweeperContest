using Minesweeper.GameServices.Contracts;

namespace Minesweeper.WebAPI.Contracts.Requests
{
    public class GetPlayerMarksResponse
    {
        public MarkTypes[,] Marks { get; set; }
    }
}