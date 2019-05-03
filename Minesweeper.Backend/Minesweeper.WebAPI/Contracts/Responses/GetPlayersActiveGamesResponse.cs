﻿using System;
using System.Collections.Generic;
using System.Linq;
using Minesweeper.GameServices.Contracts.Responses;

namespace Minesweeper.WebAPI.Contracts.Responses
{
    public class GetPlayersActiveGamesResponse
    {
        public GetPlayersActiveGamesResponse(GetPlayersActiveGamesResult queryResult)
        {
            if (queryResult == null)
            {
                throw new ArgumentNullException(nameof(queryResult));
            }

            PlayersGames = queryResult.PlayersGames.Select(x => new PlayersGame(x)).ToList();
            Total = queryResult.Total;
        }

        public IEnumerable<PlayersGame> PlayersGames { get; } = Enumerable.Empty<PlayersGame>();

        public int Total { get; }
    }
}