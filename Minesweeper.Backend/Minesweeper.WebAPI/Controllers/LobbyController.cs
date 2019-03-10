using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.GameServices.Contracts;
using Minesweeper.WebAPI.Contracts.Responses;
using Minesweeper.WebAPI.Extensions;

namespace Minesweeper.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LobbyController : ControllerBase
    {
        private readonly ILobbyService _lobbyService;

        public LobbyController(ILobbyService lobbyService)
        {
            _lobbyService = lobbyService ?? throw new ArgumentNullException(nameof(lobbyService));
        }

        [HttpGet]
        [Route("")]
        [Authorize]
        public async Task<IActionResult> Get([FromQuery]int page = 1, [FromQuery]int pageSize = 10, CancellationToken cancellationToken = default)
        {
            var userId = User.GetUserId();

            var results = await _lobbyService.GetAvailableGamesAsync(userId, page, pageSize, cancellationToken).ConfigureAwait(false);

            return Ok(new GetAvailableGamesResponse(results));
        }
    }
}