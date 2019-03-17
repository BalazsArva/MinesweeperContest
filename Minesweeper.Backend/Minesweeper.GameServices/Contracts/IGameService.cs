using System.Threading;
using System.Threading.Tasks;

namespace Minesweeper.GameServices.Contracts
{
    public interface IGameService
    {
        Task<string> StartNewGameAsync(string hostPlayerId, string hostPlayerDisplayName, string invitedPlayerId, int tableRows, int tableColumns, int mineCount, CancellationToken cancellationToken);

        Task JoinGameAsync(string gameId, string player2Id, string player2DisplayName, CancellationToken cancellationToken);

        Task<MoveResult> MakeMoveAsync(string gameId, string playerId, int row, int column, CancellationToken cancellationToken);

        Task MarkFieldAsync(string gameId, string playerId, int row, int column, MarkTypes markType, CancellationToken cancellationToken);

        Task<MarkTypes[][]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken);

        Task<VisibleFieldType[][]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken);
    }
}