using System.Threading;
using System.Threading.Tasks;

namespace Minesweeper.GameServices.Contracts
{
    public interface IGameService
    {
        Task<MarkTypes[][]> GetPlayerMarksAsync(string gameId, string playerId, CancellationToken cancellationToken);

        Task<VisibleFieldType[][]> GetVisibleGameTableAsync(string gameId, CancellationToken cancellationToken);
    }
}