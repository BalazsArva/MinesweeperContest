﻿using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Minesweeper.GameServices.Contracts.Notifications;
using Minesweeper.WebAPI.Contracts.SignalRNotifications;
using Minesweeper.WebAPI.Hubs;
using Minesweeper.WebAPI.Hubs.ClientContracts;

namespace Minesweeper.WebAPI.NotificationHandlers
{
    public class PlayerPointsChangedNotificationHandler : INotificationHandler<PlayerPointsChangedNotification>
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public PlayerPointsChangedNotificationHandler(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(PlayerPointsChangedNotification notification, CancellationToken cancellationToken)
        {
            var signalRNotification = new PlayerPointsChangedSignalRNotification(notification.PlayerId, notification.Points);

            await _hubContext.Clients.Group(notification.GameId).PointsChanged(signalRNotification).ConfigureAwait(false);
        }
    }
}