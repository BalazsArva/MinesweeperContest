using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Minesweeper.WebAPI.Contracts.SignalRRequests;
using Minesweeper.WebAPI.Hubs.ClientContracts;

namespace Minesweeper.WebAPI.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        public async Task SubscribeToGameNotifications(SubscribeToGameNotificationsRequest request)
        {
            // TODO: Uncomment this once token is provided and playerId can be read.
            /*
            var canAccessGame = await _gameService.CanAccessGameAsync(null, request.GameId, default).ConfigureAwait(false);
            if (!canAccessGame)
            {
            }
            */
            await Groups.AddToGroupAsync(Context.ConnectionId, request.GameId);
        }
    }
}