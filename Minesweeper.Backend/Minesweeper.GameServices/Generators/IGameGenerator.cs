using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.Generators
{
    public interface IGameGenerator
    {
        Game GenerateGame(int tableRows, int tableColumns, int mineCount);
    }
}