using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.GameServices.Contracts;
using Minesweeper.WebAPI.Contracts.Requests;

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

        [HttpPost]
        [Route("games", Name = RouteNames.CreateGame)]
        public async Task<IActionResult> Post([FromBody] NewGameRequest request, CancellationToken cancellationToken)
        {
            var result = await _gameService.StartNewGameAsync(request.PlayerId, request.DisplayName, request.TableRows, request.TableColumns, request.MineCount, cancellationToken).ConfigureAwait(false);

            var routeValues = new
            {
                gameId = result.GameId,
                entryToken = result.EntryToken
            };

            return CreatedAtRoute(RouteNames.Game, routeValues, null);
        }
    }
}