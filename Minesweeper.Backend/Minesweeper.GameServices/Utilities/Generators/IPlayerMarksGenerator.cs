using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.Utilities.Generators
{
    public interface IPlayerMarksGenerator
    {
        MarkTypes[][] GenerateDefaultPlayerMarksTable(int tableRows, int tableColumns);
    }
}