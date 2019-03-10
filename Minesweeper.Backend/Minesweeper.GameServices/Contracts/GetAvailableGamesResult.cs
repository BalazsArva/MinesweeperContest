using System.Collections.Generic;

namespace Minesweeper.GameServices.Contracts
{
    public class GetAvailableGamesResult
    {
        public GetAvailableGamesResult(IEnumerable<AvailableGame> availableGames, int total)
        {
            AvailableGames = availableGames;
            Total = total;
        }

        public IEnumerable<AvailableGame> AvailableGames { get; }

        public int Total { get; }
    }
}