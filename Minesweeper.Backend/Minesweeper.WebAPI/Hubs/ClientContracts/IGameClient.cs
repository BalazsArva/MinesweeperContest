using System.Threading.Tasks;
using Minesweeper.WebAPI.Contracts.SignalRNotifications;

namespace Minesweeper.WebAPI.Hubs.ClientContracts
{
    public interface IGameClient
    {
        Task GameTableUpdated(GameTableUpdated notification);
    }
}