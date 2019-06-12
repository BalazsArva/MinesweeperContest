using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Requests;
using Minesweeper.GameServices.Contracts.Responses;

namespace Minesweeper.GameServices.Handlers.RequestHandlers
{
    public interface IGetVisibleGameTableRequestHandler
    {
        Task<VisibleFieldType[][]> HandleAsync(GetVisibleGameTableRequest request, CancellationToken cancellationToken);
    }
}