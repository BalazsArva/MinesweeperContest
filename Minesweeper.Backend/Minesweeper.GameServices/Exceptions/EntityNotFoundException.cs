using System;

namespace Minesweeper.GameServices.Exceptions
{
    public class EntityNotFoundException : MinesweeperExceptionBase
    {
        public EntityNotFoundException(string message)
            : base(message)
        {
        }

        public EntityNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}