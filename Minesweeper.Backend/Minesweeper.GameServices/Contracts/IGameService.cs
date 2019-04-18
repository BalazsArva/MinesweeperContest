using System.Threading;
using System.Threading.Tasks;

namespace Minesweeper.GameServices.Contracts
{
    public interface IGameService
    {
        Task JoinGameAsync(string gameId, string player2Id, string player2DisplayName, CancellationToken cancellationToken);

        Task<MarkTypes[][]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken);

        Task<VisibleFieldType[][]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken);
    }
}