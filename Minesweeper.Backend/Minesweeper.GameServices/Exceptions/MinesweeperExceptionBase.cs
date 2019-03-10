using System;

namespace Minesweeper.GameServices.Exceptions
{
    public abstract class MinesweeperExceptionBase : Exception
    {
        public MinesweeperExceptionBase(string message)
            : base(message)
        {
        }

        public MinesweeperExceptionBase(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}