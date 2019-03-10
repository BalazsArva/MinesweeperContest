using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.GameServices.Contracts;

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
        [Route("games")]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            // TODO: Implement this
            throw new NotImplementedException();
        }
    }
}