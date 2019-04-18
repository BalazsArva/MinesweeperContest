using System;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.WebAPI.Contracts.Requests;

namespace Minesweeper.WebAPI.Mappers
{
    public static class MarkFieldMapper
    {
        public static MarkFieldCommand ToCommand(string gameId, string playerId, MarkFieldRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            return new MarkFieldCommand(gameId, playerId, request.Row, request.Column, request.MarkType);
        }
    }
}