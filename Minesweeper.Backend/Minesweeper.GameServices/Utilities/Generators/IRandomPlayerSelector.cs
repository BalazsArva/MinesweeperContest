using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.Utilities.Generators
{
    public interface IRandomPlayerSelector
    {
        Players SelectRandomPlayer();
    }
}