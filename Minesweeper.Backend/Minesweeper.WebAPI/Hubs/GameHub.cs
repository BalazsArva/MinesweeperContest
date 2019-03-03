using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Minesweeper.WebAPI.Contracts.SignalRRequests;
using Minesweeper.WebAPI.Hubs.ClientContracts;

namespace Minesweeper.WebAPI.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        public Task JoinGame(JoinGameRequest request)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, request.GameId);
        }
    }
}