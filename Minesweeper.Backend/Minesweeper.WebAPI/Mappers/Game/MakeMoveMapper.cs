using System;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.WebAPI.Contracts.Requests;

namespace Minesweeper.WebAPI.Mappers.Game
{
    public static class MakeMoveMapper
    {
        public static MakeMoveCommand ToCommand(string gameId, string playerId, MakeMoveRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new MakeMoveCommand
            {
                GameId = gameId,
                PlayerId = playerId,
                Row = request.Row,
                Column = request.Column,
            };
        }
    }
}