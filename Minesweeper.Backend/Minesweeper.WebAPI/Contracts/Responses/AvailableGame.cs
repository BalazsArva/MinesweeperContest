namespace Minesweeper.WebAPI.Contracts.Responses
{
    public class AvailableGame
    {
        public AvailableGame(GameServices.Contracts.Responses.AvailableGame serviceContract)
        {
            HostPlayerId = serviceContract.HostPlayerId;
            HostPlayerDisplayName = serviceContract.HostPlayerDisplayName;
            Rows = serviceContract.Rows;
            Columns = serviceContract.Columns;
            Mines = serviceContract.Mines;
            GameId = serviceContract.GameId;
        }

        public string GameId { get; }

        public string HostPlayerId { get; }

        public string HostPlayerDisplayName { get; }

        public int Rows { get; }

        public int Columns { get; }

        public int Mines { get; }
    }
}