using System;
using System.Security.Claims;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.WebAPI.Contracts.Requests;
using Minesweeper.WebAPI.Extensions;

namespace Minesweeper.WebAPI.Mappers
{
    public static class NewGameMapper
    {
        public static NewGameCommand ToCommand(ClaimsPrincipal user, NewGameRequest request)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new NewGameCommand
            {
                HostPlayerDisplayName = user.GetDisplayName(),
                HostPlayerId = user.GetUserId(),
                InvitedPlayerId = request.InvitedPlayerId,
                TableRows = request.TableRows,
                TableColumns = request.TableColumns,
                MineCount = request.MineCount
            };
        }
    }
}