using Minesweeper.GameServices.Contracts.Responses;
using Minesweeper.GameServices.GameModel;

namespace Minesweeper.GameServices.GameEngine
{
    public interface IGameDriver
    {
        MoveResultType MakeMove(Game game, string playerId, int row, int column);
    }
}