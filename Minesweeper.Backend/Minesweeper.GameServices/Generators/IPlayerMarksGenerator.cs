using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.Generators
{
    public interface IPlayerMarksGenerator
    {
        MarkTypes[][] GenerateDefaultPlayerMarksTable(int tableRows, int tableColumns);
    }
}