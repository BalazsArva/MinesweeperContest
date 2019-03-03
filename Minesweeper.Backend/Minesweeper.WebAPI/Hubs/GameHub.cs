using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Minesweeper.GameServices.Contracts;
using Minesweeper.WebAPI.Contracts.SignalRRequests;
using Minesweeper.WebAPI.Hubs.ClientContracts;

namespace Minesweeper.WebAPI.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        private readonly IGameService _gameService;

        public GameHub(IGameService gameService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        public Task SubscribeToGameNotifications(SubscribeToGameNotificationsRequest request)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, request.GameId);
        }
    }
}