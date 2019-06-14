using System;

namespace Minesweeper.GameServices.Utilities.Providers
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcDateTime();
    }
}