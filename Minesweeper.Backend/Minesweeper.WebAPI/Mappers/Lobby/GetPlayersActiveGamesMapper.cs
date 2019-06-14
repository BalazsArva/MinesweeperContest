using System.Linq;
using Minesweeper.GameServices.Contracts.Responses.Lobby;
using Minesweeper.WebAPI.Contracts.Responses.Lobby;

namespace Minesweeper.WebAPI.Mappers.Lobby
{
    public static class GetPlayersActiveGamesMapper
    {
        public static GetPlayersActiveGamesResponse ToApiResponse(GetPlayersActiveGamesResult serviceResult)
        {
            return new GetPlayersActiveGamesResponse
            {
                Total = serviceResult.Total,
                PlayersGames = serviceResult
                    .PlayersGames
                    .Select(serviceContract => new Contracts.Responses.Lobby.PlayersGame
                    {
                        OtherPlayerId = serviceContract.OtherPlayerId,
                        OtherPlayerDisplayName = serviceContract.OtherPlayerDisplayName,
                        Rows = serviceContract.Rows,
                        Columns = serviceContract.Columns,
                        Mines = serviceContract.Mines,
                        GameId = serviceContract.GameId
                    })
                    .ToList()
            };
        }
    }
}