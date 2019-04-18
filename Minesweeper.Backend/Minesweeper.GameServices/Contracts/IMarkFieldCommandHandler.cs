using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Commands;

namespace Minesweeper.GameServices.Contracts
{
    public interface IMarkFieldCommandHandler
    {
        Task HandleAsync(MarkFieldCommand command, CancellationToken cancellationToken);
    }
}