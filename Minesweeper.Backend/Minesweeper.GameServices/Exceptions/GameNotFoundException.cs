namespace Minesweeper.GameServices.Exceptions
{
    public class GameNotFoundException : EntityNotFoundException
    {
        public GameNotFoundException()
            : base("The requested game could not be found.")
        {
        }
    }
}