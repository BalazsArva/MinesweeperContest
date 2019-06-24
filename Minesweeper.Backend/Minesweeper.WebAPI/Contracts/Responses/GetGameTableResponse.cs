using Minesweeper.GameServices.Contracts.Responses.Game;

namespace Minesweeper.WebAPI.Contracts.Responses
{
    public class GetGameTableResponse
    {
        // TODO: Create mapper, don't expose service contract enum
        public VisibleFieldType[][] VisibleTable { get; set; }
    }
}