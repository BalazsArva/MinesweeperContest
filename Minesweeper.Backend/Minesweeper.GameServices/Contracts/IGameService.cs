using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Responses;

namespace Minesweeper.GameServices.Contracts
{
    public interface IGameService
    {
        Task<MarkTypes[][]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken);

        Task<VisibleFieldType[][]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken);
    }
}