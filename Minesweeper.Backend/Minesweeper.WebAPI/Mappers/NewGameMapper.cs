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
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new NewGameCommand
            {
                // TODO: Use display name instead of email once it is retrieved from IDP
                HostPlayerDisplayName = user.GetEmail(),
                HostPlayerId = user.GetUserId(),
                InvitedPlayerId = request.InvitedPlayerId,
                TableRows = request.TableRows,
                TableColumns = request.TableColumns,
                MineCount = request.MineCount
            };
        }
    }
}