using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.GameServices.Contracts.Responses;

namespace Minesweeper.WebAPI.Contracts.Responses
{
    public class GetAvailableGamesResponse
    {
        public GetAvailableGamesResponse(GetAvailableGamesResult queryResult)
        {
            if (queryResult == null)
            {
                throw new ArgumentNullException(nameof(queryResult));
            }

            AvailableGames = queryResult.AvailableGames.Select(x => new AvailableGame(x)).ToList();
            Total = queryResult.Total;
        }

        public IEnumerable<AvailableGame> AvailableGames { get; } = Enumerable.Empty<AvailableGame>();

        public int Total { get; }
    }
}