using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.GameServices.Contracts.Requests.Lobby;
using Minesweeper.GameServices.Handlers.RequestHandlers.Lobby;
using Minesweeper.WebAPI.Extensions;
using Minesweeper.WebAPI.Mappers.Lobby;

namespace Minesweeper.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController : ControllerBase
    {
        private readonly IGetAvailableGamesRequestHandler getAvailableGamesRequestHandler;
        private readonly IGetPlayersActiveGamesRequestHandler getPlayersActiveGamesRequestHandler;

        public LobbyController(IGetAvailableGamesRequestHandler getAvailableGamesRequestHandler, IGetPlayersActiveGamesRequestHandler getPlayersActiveGamesRequestHandler)
        {
            this.getAvailableGamesRequestHandler = getAvailableGamesRequestHandler ?? throw new ArgumentNullException(nameof(getAvailableGamesRequestHandler));
            this.getPlayersActiveGamesRequestHandler = getPlayersActiveGamesRequestHandler ?? throw new ArgumentNullException(nameof(getPlayersActiveGamesRequestHandler));
        }

        [HttpGet]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> GetAvailableGames([FromQuery]int page = 1, [FromQuery]int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var request = new GetAvailableGamesRequest
            {
                Page = page,
                PageSize = pageSize,
                UserId = User.GetUserId()
            };

            var serviceResult = await getAvailableGamesRequestHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);
            var apiResponse = GetAvailableGamesMapper.ToApiResponse(serviceResult);

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("my-games")]
        [Authorize]
        public async Task<IActionResult> GetPlayersActiveGames([FromQuery]int page = 1, [FromQuery]int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var request = new GetPlayersActiveGamesRequest
            {
                Page = page,
                PageSize = pageSize,
                UserId = User.GetUserId()
            };

            var serviceResult = await getPlayersActiveGamesRequestHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);
            var apiResponse = GetPlayersActiveGamesMapper.ToApiResponse(serviceResult);

            return Ok(apiResponse);
        }
    }
}