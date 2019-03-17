namespace Minesweeper.WebAPI.Contracts.Responses
{
    public class PlayersGame
    {
        public PlayersGame(GameServices.Contracts.PlayersGame serviceContract)
        {
            OtherPlayerId = serviceContract.OtherPlayerId;
            OtherPlayerDisplayName = serviceContract.OtherPlayerDisplayName;
            Rows = serviceContract.Rows;
            Columns = serviceContract.Columns;
            Mines = serviceContract.Mines;
            GameId = serviceContract.GameId;
        }

        public string GameId { get; }

        public string OtherPlayerId { get; }

        public string OtherPlayerDisplayName { get; }

        public int Rows { get; }

        public int Columns { get; }

        public int Mines { get; }
    }
}