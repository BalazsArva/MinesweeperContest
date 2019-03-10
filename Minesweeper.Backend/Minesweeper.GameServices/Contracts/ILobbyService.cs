using System.Threading;
using System.Threading.Tasks;

namespace Minesweeper.GameServices.Contracts
{
    public interface ILobbyService
    {
        Task<GetAvailableGamesResult> GetAvailableGamesAsync(string playerId, int page, int pageSize, CancellationToken cancellationToken);
    }
}