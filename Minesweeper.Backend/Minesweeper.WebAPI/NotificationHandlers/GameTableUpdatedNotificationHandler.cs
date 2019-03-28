﻿using System.Threading;
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

            await _hubContext.Clients.Group(notification.GameId).GameTableUpdated(signalRNotification).ConfigureAwait(false);
        }
    }

    public class RemainingMinesChangedNotificationHandler : INotificationHandler<RemainingMinesChangedNotification>
    {
        private readonly IHubContext<GameHub, IGameClient> _hubContext;

        public RemainingMinesChangedNotificationHandler(IHubContext<GameHub, IGameClient> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Handle(RemainingMinesChangedNotification notification, CancellationToken cancellationToken)
        {
            var signalRNotification = new RemainingMinesChangedSignalRNotification(notification);

            // TODO: Find out whether cancellation tokens can/should be passed to client calls
            await _hubContext.Clients.Group(notification.GameId).RemainingMinesChanged(signalRNotification).ConfigureAwait(false);
        }
    }
}