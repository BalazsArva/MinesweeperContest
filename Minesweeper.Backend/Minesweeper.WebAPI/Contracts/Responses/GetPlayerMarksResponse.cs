using Minesweeper.GameServices.Contracts;

namespace Minesweeper.WebAPI.Contracts.Responses
{
    public class GetPlayerMarksResponse
    {
        // TODO: Create mapper, don't expose service contract enum
        public MarkTypes[][] Marks { get; set; }
    }
}