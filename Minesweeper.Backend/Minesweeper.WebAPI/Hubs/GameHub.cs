using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Minesweeper.WebAPI.Hubs.ClientContracts;

namespace Minesweeper.WebAPI.Hubs
{
    public class GameHub : Hub<IGameClient>
    {
        public async Task GameTableUpdated(string gameId)
        {
            // TODO: Include the updated fields
            await Clients.Group(gameId).GameTableUpdated();
        }

        public Task JoinGame(JoinGameRequest request)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, request.GameId);
        }
    }

    public class JoinGameRequest
    {
        public string GameId { get; set; }
    }
}