namespace Minesweeper.GameServices.Contracts
{
    public class NewGameInfo
    {
        public NewGameInfo(string gameId, string entryToken)
        {
            GameId = gameId;
            EntryToken = entryToken;
        }

        public string GameId { get; }

        public string EntryToken { get; }
    }
}