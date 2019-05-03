namespace Minesweeper.GameServices.Contracts.Responses
{
    public class AvailableGame
    {
        public AvailableGame(string gameId, string hostPlayerId, string hostPlayerDisplayName, int rows, int columns, int mines)
        {
            GameId = gameId;
            HostPlayerId = hostPlayerId;
            HostPlayerDisplayName = hostPlayerDisplayName;
            Rows = rows;
            Columns = columns;
            Mines = mines;
        }

        public string GameId { get; }

        public string HostPlayerId { get; }

        public string HostPlayerDisplayName { get; }

        public int Rows { get; }

        public int Columns { get; }

        public int Mines { get; }
    }
}