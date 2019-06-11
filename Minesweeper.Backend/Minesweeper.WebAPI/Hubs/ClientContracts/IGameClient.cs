using System.Threading.Tasks;
using Minesweeper.WebAPI.Contracts.SignalRNotifications;

namespace Minesweeper.WebAPI.Hubs.ClientContracts
{
    public interface IGameClient
    {
        Task GameTableUpdated(GameTableUpdatedSignalRNotification notification);

        Task RemainingMinesChanged(RemainingMinesChangedSignalRNotification notification);

        Task GameOver(GameOverSignalRNotification notification);

        Task PointsChanged(PlayerPointsChangedSignalRNotification notification);

        Task TurnChanged(PlayersTurnSignalRNotification notification);
    }
}