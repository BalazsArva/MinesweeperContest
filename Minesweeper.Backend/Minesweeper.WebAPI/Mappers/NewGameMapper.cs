using System;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.WebAPI.Contracts.Requests;

namespace Minesweeper.WebAPI.Mappers
{
    public static class NewGameMapper
    {
        public static NewGameCommand ToCommand(string playerId, NewGameRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new NewGameCommand
            {
                HostPlayerId = playerId,
                InvitedPlayerId = request.InvitedPlayerId,
                TableRows = request.TableRows,
                TableColumns = request.TableColumns,
                MineCount = request.MineCount
            };
        }
    }
}