namespace Minesweeper.GameServices.GameModel
{
    public class Player
    {
        public Player(string playerId, string displayName)
        {
            PlayerId = playerId;
            DisplayName = displayName;
        }

        public string PlayerId { get; }

        public string DisplayName { get; }
    }
}