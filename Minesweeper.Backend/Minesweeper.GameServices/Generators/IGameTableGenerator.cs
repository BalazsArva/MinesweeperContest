using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.Generators
{
    public interface IGameTableGenerator
    {
        GameTable GenerateGameTable(int rows, int columns, int mineCount);
    }
}