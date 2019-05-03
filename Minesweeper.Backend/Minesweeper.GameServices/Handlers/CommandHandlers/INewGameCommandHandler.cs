using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Commands;

namespace Minesweeper.GameServices.Handlers.CommandHandlers
{
    public interface INewGameCommandHandler
    {
        Task<string> HandleAsync(NewGameCommand command, CancellationToken cancellationToken);
    }
}