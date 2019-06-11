namespace Minesweeper.GameServices.GameModel
{
    public class Player
    {
        public string PlayerId { get; set; }

        public string DisplayName { get; set; }

        public int Points { get; set; }

        public int StreakBonus { get; set; }
    }
}