using System.Threading;
using System.Threading.Tasks;

namespace Minesweeper.GameServices.Contracts
{
    public interface IGameService
    {
        Task<bool> CanAccessGameAsync(string playerId, string gameId, CancellationToken cancellationToken);

        Task<NewGameInfo> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, int tableRows, int tableColumns, int mineCount, CancellationToken cancellationToken);

        Task JoinGameAsync(string gameId, string player2Id, string player2DisplayName, CancellationToken cancellationToken);

        Task<MoveResult> MakeMoveAsync(string gameId, string playerId, int row, int column, CancellationToken cancellationToken);

        Task<VisibleFieldType[,]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken);
    }
}