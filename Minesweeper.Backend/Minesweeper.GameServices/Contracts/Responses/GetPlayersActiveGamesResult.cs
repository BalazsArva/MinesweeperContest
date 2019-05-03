using System.Collections.Generic;

namespace Minesweeper.GameServices.Contracts.Responses
{
    public class GetPlayersActiveGamesResult
    {
        public GetPlayersActiveGamesResult(IEnumerable<PlayersGame> playersGames, int total)
        {
            PlayersGames = playersGames;
            Total = total;
        }

        public IEnumerable<PlayersGame> PlayersGames { get; }

        public int Total { get; }
    }
}