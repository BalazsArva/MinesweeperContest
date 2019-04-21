using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Commands;

namespace Minesweeper.GameServices.Contracts
{
    public interface IJoinGameCommandHandler
    {
        Task HandleAsync(JoinGameCommand command, CancellationToken cancellationToken);
    }
}