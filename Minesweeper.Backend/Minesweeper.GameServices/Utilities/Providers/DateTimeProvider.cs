using System;

namespace Minesweeper.GameServices.Utilities.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcDateTime() => DateTime.UtcNow;
    }
}