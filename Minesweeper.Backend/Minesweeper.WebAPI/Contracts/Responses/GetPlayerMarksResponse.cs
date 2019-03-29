using Minesweeper.GameServices.Contracts;

namespace Minesweeper.WebAPI.Contracts.Responses
{
    public class GetPlayerMarksResponse
    {
        public GetPlayerMarksResponse(MarkTypes[][] marks)
        {
            Marks = marks;
        }

        public MarkTypes[][] Marks { get; }
    }
}