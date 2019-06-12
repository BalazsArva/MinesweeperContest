using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Requests;
using Minesweeper.GameServices.Contracts.Responses;

namespace Minesweeper.GameServices.Handlers.RequestHandlers
{
    public interface IGetGameStateRequestHandler
    {
        Task<GetGameStateResponse> HandleAsync(GetGameStateRequest request, CancellationToken cancellationToken);
    }
}