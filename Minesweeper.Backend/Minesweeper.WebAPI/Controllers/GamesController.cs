﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minesweeper.GameServices.Contracts;
using Minesweeper.WebAPI.Contracts.Requests;
using Minesweeper.WebAPI.Extensions;

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

        [HttpPost]
        [Route("{gameId}/make-move")]
        [Authorize]
        public async Task<IActionResult> MakeMove(string gameId, [FromBody]MakeMoveRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();

            // TODO: This is only temp
            await _gameService.MakeMoveAsync(gameId, userId, request.Row, request.Column, cancellationToken).ConfigureAwait(false);

            // TODO: Error handling
            return Ok();
        }

        [HttpPost]
        [Route("{gameId}/mark-field")]
        [Authorize]
        public async Task<IActionResult> MarkField(string gameId, [FromBody]MarkFieldRequest request, CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();

            // TODO: This is only temp
            // TODO: Expose a separate enum type for mark type
            await _gameService.MarkFieldAsync(gameId, userId, request.Row, request.Column, request.MarkType, cancellationToken).ConfigureAwait(false);

            // TODO: Error handling
            return Ok();
        }

        [HttpPost]
        [Route("", Name = RouteNames.CreateGame)]
        [Authorize]
        public async Task<IActionResult> CreateGame([FromBody] NewGameRequest request, CancellationToken cancellationToken)
        {
            // TODO: Retrieve the real display name
            var userId = User.GetUserId();
            var displayName = userId;

            var result = await _gameService.StartNewGameAsync(userId, displayName, request.InvitedPlayerId, request.TableRows, request.TableColumns, request.MineCount, cancellationToken).ConfigureAwait(false);

            var routeValues = new
            {
                gameId = result
            };

            // TODO: Provide proper route once it is finalized
            return CreatedAtRoute(RouteNames.Game, routeValues, null);
        }

        [HttpPost]
        [Route("{gameId}/player2", Name = RouteNames.JoinGame)]
        [Authorize]
        public async Task<IActionResult> JoinGame(string gameId, CancellationToken cancellationToken)
        {
            // TODO: Retrieve the real display name
            var userId = User.GetUserId();
            var displayName = userId;

            await _gameService.JoinGameAsync(gameId, userId, displayName, cancellationToken).ConfigureAwait(false);

            var routeValues = new { gameId };

            return CreatedAtRoute(RouteNames.Game, routeValues, null);
        }
    }
}