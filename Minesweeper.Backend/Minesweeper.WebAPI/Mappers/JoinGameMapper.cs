using System;
using System.Security.Claims;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.WebAPI.Extensions;

namespace Minesweeper.WebAPI.Mappers
{
    public static class JoinGameMapper
    {
        public static JoinGameCommand ToCommand(ClaimsPrincipal user, string gameId)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            return new JoinGameCommand
            {
                GameId = gameId,
                PlayerId = user.GetUserId(),
                PlayerDisplayName = user.GetDisplayName(),
            };
        }
    }
}