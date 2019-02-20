using System;

namespace Minesweeper.GameServices.Providers
{
    public interface IDateTimeProvider
    {
        DateTime GetUtcDateTime();
    }
}