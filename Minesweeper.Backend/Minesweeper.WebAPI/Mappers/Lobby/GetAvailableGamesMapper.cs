using System.Linq;
using Minesweeper.GameServices.Contracts.Responses.Lobby;
using Minesweeper.WebAPI.Contracts.Responses.Lobby;

namespace Minesweeper.WebAPI.Mappers.Lobby
{
    public static class GetAvailableGamesMapper
    {
        public static GetAvailableGamesResponse ToApiResponse(GetAvailableGamesResult serviceResult)
        {
            return new GetAvailableGamesResponse
            {
                Total = serviceResult.Total,
                AvailableGames = serviceResult
                    .AvailableGames
                    .Select(x => new Contracts.Responses.Lobby.AvailableGame
                    {
                        HostPlayerId = x.HostPlayerId,
                        HostPlayerDisplayName = x.HostPlayerDisplayName,
                        Rows = x.Rows,
                        Columns = x.Columns,
                        Mines = x.Mines,
                        GameId = x.GameId
                    })
                    .ToList()
            };
        }
    }
}