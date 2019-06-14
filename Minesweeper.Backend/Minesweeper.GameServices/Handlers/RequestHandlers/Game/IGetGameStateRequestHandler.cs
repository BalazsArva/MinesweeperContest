using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Requests.Game;
using Minesweeper.GameServices.Contracts.Responses.Game;

namespace Minesweeper.GameServices.Handlers.RequestHandlers.Game
{
    public interface IGetGameStateRequestHandler
    {
        Task<GetGameStateResponse> HandleAsync(GetGameStateRequest request, CancellationToken cancellationToken);
    }
}