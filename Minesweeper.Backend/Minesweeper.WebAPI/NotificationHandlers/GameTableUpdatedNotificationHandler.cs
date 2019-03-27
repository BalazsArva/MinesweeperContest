using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Minesweeper.GameServices.Contracts;
using Minesweeper.WebAPI.Contracts.SignalRNotifications;
using Minesweeper.WebAPI.Hubs;
using Minesweeper.WebAPI.Hubs.ClientContracts;

namespace Minesweeper.WebAPI.NotificationHandlers
{
    public class GameTableUpdatedNotificationHandler : INotificationHandler<GameTableUpdatedNotification>
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public GameTableUpdatedNotificationHandler(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(GameTableUpdatedNotification notification, CancellationToken cancellationToken)
        {
            var signalRNotification = new GameTableUpdatedSignalRNotification(notification.FieldUpdates);

            await _hubContext.Clients.Group(notification.GameId).GameTableUpdated(signalRNotification);
        }
    }
}