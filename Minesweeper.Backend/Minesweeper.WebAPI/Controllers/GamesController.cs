using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.GameServices.Contracts.Requests.Game;
using Minesweeper.GameServices.Handlers.CommandHandlers;
using Minesweeper.GameServices.Handlers.RequestHandlers.Game;
using Minesweeper.WebAPI.Contracts.Requests;
using Minesweeper.WebAPI.Contracts.Responses;
using Minesweeper.WebAPI.Extensions;
using Minesweeper.WebAPI.Mappers;
using Minesweeper.WebAPI.Mappers.Game;

namespace Minesweeper.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly IGetGameStateRequestHandler _getGameStateRequestHandler;
        private readonly IGetVisibleGameTableRequestHandler _getVisibleGameTableRequestHandler;
        private readonly IGetPlayerMarksRequestHandler _getPlayerMarksRequestHandler;
        private readonly IMakeMoveCommandHandler _makeMoveCommandHandler;
        private readonly IJoinGameCommandHandler _joinGameCommandHandler;
        private readonly IMarkFieldCommandHandler _markFieldCommandHandler;
        private readonly INewGameCommandHandler _newGameCommandHandler;

        public GamesController(
            IGetGameStateRequestHandler getGameStateRequestHandler,
            IGetVisibleGameTableRequestHandler getVisibleGameTableRequestHandler,
            IGetPlayerMarksRequestHandler getPlayerMarksRequestHandler,
            IMakeMoveCommandHandler makeMoveCommandHandler,
            IJoinGameCommandHandler joinGameCommandHandler,
            IMarkFieldCommandHandler markFieldCommandHandler,
            INewGameCommandHandler newGameCommandHandler)
        {
            _getGameStateRequestHandler = getGameStateRequestHandler ?? throw new ArgumentNullException(nameof(getGameStateRequestHandler));
            _getVisibleGameTableRequestHandler = getVisibleGameTableRequestHandler ?? throw new ArgumentNullException(nameof(getVisibleGameTableRequestHandler));
            _getPlayerMarksRequestHandler = getPlayerMarksRequestHandler ?? throw new ArgumentNullException(nameof(getPlayerMarksRequestHandler));
            _makeMoveCommandHandler = makeMoveCommandHandler ?? throw new ArgumentNullException(nameof(makeMoveCommandHandler));
            _joinGameCommandHandler = joinGameCommandHandler ?? throw new ArgumentNullException(nameof(joinGameCommandHandler));
            _markFieldCommandHandler = markFieldCommandHandler ?? throw new ArgumentNullException(nameof(markFieldCommandHandler));
            _newGameCommandHandler = newGameCommandHandler ?? throw new ArgumentNullException(nameof(newGameCommandHandler));
        }

        [HttpGet]
        [Route("{gameId}", Name = RouteNames.Game)]
        [Authorize]
        public async Task<IActionResult> Get(string gameId, CancellationToken cancellationToken)
        {
            var serviceResponse = await _getGameStateRequestHandler.HandleAsync(new GetGameStateRequest { GameId = gameId }, cancellationToken).ConfigureAwait(false);

            var apiResponse = GetGameStateResponseMapper.ToApiResponse(serviceResponse);

            return Ok(apiResponse);
        }

        [HttpGet]
        [Route("{gameId}/table")]
        [Authorize]
        public async Task<IActionResult> GetGameTableTemp(string gameId, CancellationToken cancellationToken)
        {
            // TODO: This is only temp
            var request = new GetVisibleGameTableRequest { GameId = gameId };

            var result = await _getVisibleGameTableRequestHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);

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
            var request = new GetPlayerMarksRequest
            {
                GameId = gameId,
                PlayerId = User.GetUserId()
            };

            var playerMarks = await _getPlayerMarksRequestHandler.HandleAsync(request, cancellationToken).ConfigureAwait(false);

            return Ok(new GetPlayerMarksResponse
            {
                Marks = playerMarks
            });
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
            var command = NewGameMapper.ToCommand(User, request);

            var gameId = await _newGameCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            // TODO: Provide proper route once it is finalized
            return CreatedAtRoute(RouteNames.Game, new { gameId }, null);
        }

        [HttpPost]
        [Route("{gameId}/player2", Name = RouteNames.JoinGame)]
        [Authorize]
        public async Task<IActionResult> JoinGame(string gameId, CancellationToken cancellationToken)
        {
            var command = JoinGameMapper.ToCommand(User, gameId);

            await _joinGameCommandHandler.HandleAsync(command, cancellationToken).ConfigureAwait(false);

            var routeValues = new { gameId };

            return CreatedAtRoute(RouteNames.Game, routeValues, null);
        }
    }
}