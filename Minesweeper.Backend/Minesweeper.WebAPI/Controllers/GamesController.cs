using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.Contracts.Commands;
using Minesweeper.WebAPI.Contracts.Requests;
using Minesweeper.WebAPI.Contracts.Responses;
using Minesweeper.WebAPI.Extensions;
using Minesweeper.WebAPI.Mappers;

namespace Minesweeper.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly IMakeMoveCommandHandler _makeMoveCommandHandler;
        private readonly IJoinGameCommandHandler _joinGameCommandHandler;
        private readonly IMarkFieldCommandHandler _markFieldCommandHandler;
        private readonly INewGameCommandHandler _newGameCommandHandler;

        public GamesController(IGameService gameService, IMakeMoveCommandHandler makeMoveCommandHandler, IJoinGameCommandHandler joinGameCommandHandler, IMarkFieldCommandHandler markFieldCommandHandler, INewGameCommandHandler newGameCommandHandler)
        {
            _gameService = gameService ?? throw new ArgumentNullException(nameof(gameService));
            _makeMoveCommandHandler = makeMoveCommandHandler ?? throw new ArgumentNullException(nameof(makeMoveCommandHandler));
            _joinGameCommandHandler = joinGameCommandHandler ?? throw new ArgumentNullException(nameof(joinGameCommandHandler));
            _markFieldCommandHandler = markFieldCommandHandler ?? throw new ArgumentNullException(nameof(markFieldCommandHandler));
            _newGameCommandHandler = newGameCommandHandler ?? throw new ArgumentNullException(nameof(newGameCommandHandler));
        }

        [HttpGet]
        [Route("{gameId}", Name = RouteNames.Game)]
        public async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        [HttpGet]
        [Route("{gameId}/table")]
        [Authorize]
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

        [HttpGet]
        [Route("{gameId}/player-marks")]
        [Authorize]
        public async Task<IActionResult> GetPlayerMarks(string gameId, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();

            var playerMarks = await _gameService.GetPlayerMarksAsync(gameId, userId, cancellationToken).ConfigureAwait(false);

            return Ok(new GetPlayerMarksResponse(playerMarks));
        }

        [HttpPost]
        [Route("{gameId}/make-move")]
        [Authorize]
        public async Task<IActionResult> MakeMove(string gameId, [FromBody]MakeMoveRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = MakeMoveMapper.ToCommand(gameId, userId, request);

            await _makeMoveCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            // TODO: Error handling
            return Ok();
        }

        [HttpPost]
        [Route("{gameId}/mark-field")]
        [Authorize]
        public async Task<IActionResult> MarkField(string gameId, [FromBody]MarkFieldRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = MarkFieldMapper.ToCommand(gameId, userId, request);

            await _markFieldCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            // TODO: Error handling
            return Ok();
        }

        [HttpPost]
        [Route("", Name = RouteNames.CreateGame)]
        [Authorize]
        public async Task<IActionResult> CreateGame([FromBody] NewGameRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var command = NewGameMapper.ToCommand(userId, request);

            var gameId = await _newGameCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            // TODO: Provide proper route once it is finalized
            return CreatedAtRoute(RouteNames.Game, new { gameId }, null);
        }

        [HttpPost]
        [Route("{gameId}/player2", Name = RouteNames.JoinGame)]
        [Authorize]
        public async Task<IActionResult> JoinGame(string gameId, CancellationToken cancellationToken)
        {
            // TODO: Retrieve the real display name
            var userId = User.GetUserId();
            var displayName = userId;

            var command = new JoinGameCommand(gameId, userId, displayName);

            await _joinGameCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            var routeValues = new { gameId };

            return CreatedAtRoute(RouteNames.Game, routeValues, null);
        }
    }
}