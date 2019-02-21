using System;

namespace Minesweeper.GameServices.GameModel
{
    public class GameMove
    {
        public Players Player { get; set; }

        public int Row { get; set; }

        public int Column { get; set; }

        public DateTime UtcDateTimeRecorded { get; set; }
    }
}