using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices
{
    public interface IGameDriver
    {
        MoveResultType MakeMove(Game game, string playerId, int row, int column);
    }
}