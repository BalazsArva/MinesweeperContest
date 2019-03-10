using System;
using System.Collections.Generic;

namespace Minesweeper.GameServices.GameModel
{
    public class Game
    {
        public string Id { get; set; }

        public DateTime? UtcDateTimeStarted { get; set; }

        public GameTable GameTable { get; set; }

        public Players StarterPlayer { get; set; }

        public Players? Winner { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public GameStatus Status { get; set; }

        public string InvitedPlayerId { get; set; }

        public List<GameMove> Moves { get; set; } = new List<GameMove>();
    }
}