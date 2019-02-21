using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices
{
    public interface IGameTableVisibilityComputer
    {
        VisibleFieldType[,] GetVisibleGameTableAsync(Game game);
    }
}