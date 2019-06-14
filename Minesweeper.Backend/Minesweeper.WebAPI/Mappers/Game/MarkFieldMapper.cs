using System;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.WebAPI.Contracts.Requests;

namespace Minesweeper.WebAPI.Mappers.Game
{
    public static class MarkFieldMapper
    {
        public static MarkFieldCommand ToCommand(string gameId, string playerId, MarkFieldRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new MarkFieldCommand
            {
                GameId = gameId,
                PlayerId = playerId,
                Row = request.Row,
                Column = request.Column,
                MarkType = request.MarkType
            };
        }
    }
}