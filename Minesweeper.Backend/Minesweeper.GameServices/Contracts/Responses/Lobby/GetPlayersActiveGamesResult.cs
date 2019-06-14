using System.Collections.Generic;

namespace Minesweeper.GameServices.Contracts.Responses.Lobby
{
    public class GetPlayersActiveGamesResult
    {
        public IEnumerable<PlayersGame> PlayersGames { get; set; }

        public int Total { get; set; }
    }
}