using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Responses;

namespace Minesweeper.GameServices.Contracts
{
    public interface ILobbyService
    {
        Task<GetAvailableGamesResult> GetAvailableGamesAsync(string playerId, int page, int pageSize, CancellationToken cancellationToken);

        Task<GetPlayersActiveGamesResult> GetPlayersActiveGamesAsync(string userId, int page, int pageSize, CancellationToken cancellationToken);
    }
}