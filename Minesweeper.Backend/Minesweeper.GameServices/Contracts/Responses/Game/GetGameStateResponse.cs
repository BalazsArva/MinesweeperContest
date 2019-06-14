using System;

namespace Minesweeper.GameServices.Contracts.Responses.Game
{
    public class GetGameStateResponse
    {
        public int RemainingMines { get; set; }

        public DateTime? UtcDateTimeStarted { get; set; }

        public Players NextPlayer { get; set; }

        public Players? Winner { get; set; }

        public GameStatus Status { get; set; }

        public PlayerState Player1State { get; set; }

        public PlayerState Player2State { get; set; }
    }
}