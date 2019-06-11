using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Minesweeper.GameServices.Contracts.Notifications;
using Minesweeper.WebAPI.Contracts.SignalRNotifications;
using Minesweeper.WebAPI.Hubs;
using Minesweeper.WebAPI.Hubs.ClientContracts;

namespace Minesweeper.WebAPI.NotificationHandlers
{
    public class PlayersTurnNotificationHandler : INotificationHandler<PlayersTurnNotification>
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public PlayersTurnNotificationHandler(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(PlayersTurnNotification notification, CancellationToken cancellationToken)
        {
            var signalRNotification = new PlayersTurnSignalRNotification(notification.PlayerId);

            await _hubContext.Clients.Group(notification.GameId).TurnChanged(signalRNotification).ConfigureAwait(false);
        }
    }
}