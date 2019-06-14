using System.Collections.Generic;

namespace Minesweeper.WebAPI.Contracts.Responses.Lobby
{
    public class GetAvailableGamesResponse
    {
        public IEnumerable<AvailableGame> AvailableGames { get; set; }

        public int Total { get; set; }
    }
}