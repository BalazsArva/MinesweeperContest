using System;

namespace Minesweeper.GameServices.Providers
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetUtcDateTime() => DateTime.UtcNow;
    }
}