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
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;

        public GamesController(IGameService gameService)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
        }

        [HttpGet]
        [Route("{gameId}", Name = RouteNames.Game)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{gameId}/table")]
        public async Task<IActionResult> GetGameTableTemp(string gameId, CancellationToken cancellationToken)
        {
            // TODO: This is only temp
            var result = await _gameService.GetVisibleGameTableAsync(gameId, cancellationToken).ConfigureAwait(false);

            // TODO: Error handling
            return Ok(new
            {
                VisibleTable = result
            });
        }

        [HttpPost]
        [Route("{gameId}/movement")]
        public async Task<IActionResult> GetGameTableTemp(string gameId, [FromBody]MakeMoveRequest request, CancellationToken cancellationToken)
        {
            // TODO: This is only temp
            await _gameService.MakeMoveAsync(gameId, request.PlayerId, request.Row, request.Column, cancellationToken).ConfigureAwait(false);

            // TODO: Error handling
            return Ok();
        }
    }
}