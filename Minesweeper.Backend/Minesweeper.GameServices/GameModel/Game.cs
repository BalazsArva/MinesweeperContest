using System;

namespace Minesweeper.GameServices.GameModel
{
    public class Game
    {
        public string Id { get; set; }

        public DateTime? UtcDateTimeStarted { get; set; }

        public Players NextPlayer { get; set; }

        public Players? Winner { get; set; }

        public Player Player1 { get; set; }

        public Player Player2 { get; set; }

        public GameStatus Status { get; set; }

        public string InvitedPlayerId { get; set; }

        public int Rows { get; set; }

        public int Columns { get; set; }

        public int Mines { get; set; }

        public FieldTypes[][] BaseTable { get; set; }

        public VisibleFieldType[][] VisibleTable { get; set; }

        public MarkTypes[][] Player1Marks { get; set; }

        public MarkTypes[][] Player2Marks { get; set; }
    }
}