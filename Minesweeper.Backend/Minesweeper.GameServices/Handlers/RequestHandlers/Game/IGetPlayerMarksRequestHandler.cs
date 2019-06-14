using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Requests.Game;

namespace Minesweeper.GameServices.Handlers.RequestHandlers.Game
{
    public interface IGetPlayerMarksRequestHandler
    {
        Task<MarkTypes[][]> HandleAsync(GetPlayerMarksRequest request, CancellationToken cancellationToken);
    }
}