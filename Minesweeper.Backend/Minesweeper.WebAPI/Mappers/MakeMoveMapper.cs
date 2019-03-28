using System;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.WebAPI.Contracts.Requests;

namespace Minesweeper.WebAPI.Mappers
{
    public static class MakeMoveMapper
    {
        public static MakeMoveCommand ToCommand(string gameId, string playerId, MakeMoveRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new MakeMoveCommand(gameId, playerId, request.Row, request.Column);
        }
    }
}