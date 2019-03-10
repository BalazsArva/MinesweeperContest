using System;

namespace Minesweeper.GameServices.Exceptions
{
    public class ActionNotAllowedException : MinesweeperExceptionBase
    {
        public ActionNotAllowedException(string message)
            : base(message)
        {
        }

        public ActionNotAllowedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}