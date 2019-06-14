using System.Collections.Generic;

namespace Minesweeper.WebAPI.Contracts.Responses.Lobby
{
    public class GetPlayersActiveGamesResponse
    {
        public IEnumerable<PlayersGame> PlayersGames { get; set; }

        public int Total { get; set; }
    }
}