using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Requests.Game;
using Minesweeper.GameServices.Contracts.Responses.Game;

namespace Minesweeper.GameServices.Handlers.RequestHandlers.Game
{
    public interface IGetVisibleGameTableRequestHandler
    {
        Task<VisibleFieldType[][]> HandleAsync(GetVisibleGameTableRequest request, CancellationToken cancellationToken);
    }
}