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
    public class GameOverNotificationHandler : INotificationHandler<GameOverNotification>
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public GameOverNotificationHandler(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(GameOverNotification notification, CancellationToken cancellationToken)
        {
            var signalRNotification = new GameOverSignalRNotification(notification.WinnerPlayerId);

            await _hubContext.Clients.Group(notification.GameId).GameOver(signalRNotification).ConfigureAwait(false);
        }
    }
}