namespace Minesweeper.GameServices.Contracts
{
    public class PlayersGame
    {
        public PlayersGame(string gameId, string otherPlayerId, string otherPlayerDisplayName, int rows, int columns, int mines)
        {
            GameId = gameId;
            OtherPlayerId = otherPlayerId;
            OtherPlayerDisplayName = otherPlayerDisplayName;
            Rows = rows;
            Columns = columns;
            Mines = mines;
        }

        public string GameId { get; }

        public string OtherPlayerId { get; }

        public string OtherPlayerDisplayName { get; }

        public int Rows { get; }

        public int Columns { get; }

        public int Mines { get; }
    }
}