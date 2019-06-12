using System;

namespace Minesweeper.GameServices.Contracts.Responses
{
    public class GetGameStateResponse
    {
        public int RemainingMines { get; set; }

        public DateTime? UtcDateTimeStarted { get; set; }

        public Players NextPlayer { get; set; }

        public Players? Winner { get; set; }

        public GameStatus Status { get; set; }
    }
}