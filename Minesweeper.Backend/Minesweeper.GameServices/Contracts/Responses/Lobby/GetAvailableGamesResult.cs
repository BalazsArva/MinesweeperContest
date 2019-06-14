using System.Collections.Generic;

namespace Minesweeper.GameServices.Contracts.Responses.Lobby
{
    public class GetAvailableGamesResult
    {
        public IEnumerable<AvailableGame> AvailableGames { get; set; }

        public int Total { get; set; }
    }
}