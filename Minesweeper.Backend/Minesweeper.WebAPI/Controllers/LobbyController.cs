using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.GameServices.Contracts;
using Minesweeper.WebAPI.Contracts.Requests;
using Minesweeper.WebAPI.Contracts.Responses;

namespace Minesweeper.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController : ControllerBase
    {
        private readonly IGameService _gameService;

        public LobbyController(IGameService gameService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        [HttpGet]
        [Route("anonymous-id")]
        public IActionResult RequestAnonymousPlayerId()
        {
            return Ok(new GetAnonymousPlayerIdResponse { PlayerId = Guid.NewGuid().ToString() });
        }

        [HttpGet]
        [Route("games")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var player1Id = "68dbcce5-eb47-4e1f-928d-4709bc0811e8";
            var player2Id = "24341538-9afb-4ae8-b90e-baa85cac57b5";

            var result = await _gameService.StartNewGameAsync(player1Id, "Balazs", 16, 32, 99, cancellationToken).ConfigureAwait(false);

            await _gameService.JoinGameAsync(result.GameId, player2Id, "OtherPlayer", result.EntryToken, cancellationToken).ConfigureAwait(false);

            await _gameService.MakeMoveAsync(result.GameId, player1Id, 0, 0, cancellationToken).ConfigureAwait(false);

            var routeValues = new
            {
                gameId = result.GameId,
                entryToken = result.EntryToken
            };

            return CreatedAtRoute(RouteNames.Game, routeValues, null);
        }

        [HttpPost]
        [Route("games", Name = RouteNames.CreateGame)]
        public async Task<IActionResult> CreateGame([FromBody] NewGameRequest request, CancellationToken cancellationToken)
        {
            var result = await _gameService.StartNewGameAsync(request.PlayerId, request.DisplayName, request.TableRows, request.TableColumns, request.MineCount, cancellationToken).ConfigureAwait(false);

            var routeValues = new
            {
                gameId = result.GameId,
                entryToken = result.EntryToken
            };

            return CreatedAtRoute(RouteNames.Game, routeValues, null);
        }

        [HttpPost]
        [Route("game/{gameId}", Name = RouteNames.JoinGame)]
        public async Task<IActionResult> JoinGame(string gameId, [FromBody]JoinGameRequest request, CancellationToken cancellationToken)
        {
            await _gameService.JoinGameAsync(gameId, request.PlayerId, request.DisplayName, request.EntryToken, cancellationToken).ConfigureAwait(false);

            var routeValues = new { gameId };

            return CreatedAtRoute(RouteNames.Game, routeValues, null);
        }
    }
}