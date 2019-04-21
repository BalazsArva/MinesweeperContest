namespace Minesweeper.GameServices.Contracts.Commands
{
    public class JoinGameCommand
    {
        public JoinGameCommand(string gameId, string playerId, string playerDisplayName)
        {
            GameId = gameId;
            PlayerId = playerId;
            PlayerDisplayName = playerDisplayName;
        }

        public string GameId { get; }

        public string PlayerId { get; }

        public string PlayerDisplayName { get; }
    }
}