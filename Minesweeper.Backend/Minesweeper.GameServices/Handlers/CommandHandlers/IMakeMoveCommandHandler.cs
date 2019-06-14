using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.GameServices.Contracts.Responses.Game;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public interface IMakeMoveCommandHandler
    {
        Task<MoveResult> HandleAsync(MakeMoveCommand command, CancellationToken cancellationToken);
    }
}