using Minesweeper.GameServices.Contracts.Responses;

namespace Minesweeper.GameServices.Contracts.Commands
{
    public class MarkFieldCommand
    {
        public MarkFieldCommand(string gameId, string playerId, int row, int column, MarkTypes markType)
        {
            GameId = gameId;
            PlayerId = playerId;
            Row = row;
            Column = column;
            MarkType = markType;
        }

        public string GameId { get; }

        public string PlayerId { get; }

        public int Row { get; }

        public int Column { get; }

        public MarkTypes MarkType { get; }
    }
}