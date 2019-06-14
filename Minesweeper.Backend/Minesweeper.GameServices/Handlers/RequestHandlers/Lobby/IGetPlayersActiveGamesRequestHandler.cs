using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Requests.Lobby;
using Minesweeper.GameServices.Contracts.Responses.Lobby;

namespace Minesweeper.GameServices.Handlers.RequestHandlers.Lobby
{
    public interface IGetPlayersActiveGamesRequestHandler
    {
        Task<GetPlayersActiveGamesResult> HandleAsync(GetPlayersActiveGamesRequest request, CancellationToken cancellationToken);
    }
}