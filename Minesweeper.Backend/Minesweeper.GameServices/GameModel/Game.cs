using System;
using System.Collections.Generic;

namespace Minesweeper.GameServices.GameModel
{
    public class Game
    {
        public string Id { get; set; }

        public DateTime UtcDateTimeStarted { get; set; }

        public string EntryToken { get; set; }

        public GameTable GameTable { get; set; }

        public Players StarterPlayer { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public List<GameMove> Moves { get; set; } = new List<GameMove>();
    }
}