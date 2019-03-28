using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Commands;

namespace Minesweeper.GameServices.Contracts
{
    public interface IMakeMoveCommandHandler
    {
        Task<MoveResult> HandleAsync(MakeMoveCommand command, CancellationToken cancellationToken);
    }
}