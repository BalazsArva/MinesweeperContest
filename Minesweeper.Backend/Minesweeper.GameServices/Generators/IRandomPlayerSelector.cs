using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.Generators
{
    public interface IRandomPlayerSelector
    {
        Players SelectRandomPlayer();
    }
}