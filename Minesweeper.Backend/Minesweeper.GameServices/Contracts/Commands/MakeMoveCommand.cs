namespace Minesweeper.GameServices.Contracts.Commands
{
    public class MakeMoveCommand
    {
        public MakeMoveCommand(string gameId, string playerId, int row, int column)
        {
            GameId = gameId;
            PlayerId = playerId;
            Row = row;
            Column = column;
        }

        public string GameId { get; }

        public string PlayerId { get; }

        public int Row { get; }

        public int Column { get; }
    }
}