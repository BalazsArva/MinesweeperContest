using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.GameServices.Contracts.Requests.Game;
using Minesweeper.GameServices.Contracts.Responses.Game;
using Minesweeper.GameServices.Exceptions;
using Minesweeper.GameServices.Extensions;
using Raven.Client.Documents;

namespace Minesweeper.GameServices.Handlers.RequestHandlers.Game
{
    public class GetGameStateRequestHandler : IGetGameStateRequestHandler
    {
        private readonly IDocumentStore _documentStore;

        public GetGameStateRequestHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<GetGameStateResponse> HandleAsync(GetGameStateRequest request, CancellationToken cancellationToken)
        {
            using (var session = _documentStore.OpenAsyncSession())
            {
                var gameDocument = await session.LoadGameAsync(request.GameId, cancellationToken).ConfigureAwait(false);

                // TODO: Validate that the requesting user is a participant of the game
                if (gameDocument == null)
                {
                    throw new GameNotFoundException();
                }

                var totalMines = gameDocument.Mines;
                var foundMines = gameDocument
                    .VisibleTable
                    .SelectMany(arr => arr)
                    .Count(field => field == GameModel.VisibleFieldType.Player1FoundMine || field == GameModel.VisibleFieldType.Player2FoundMine);

                return new GetGameStateResponse
                {
                    RemainingMines = totalMines - foundMines,
                    UtcDateTimeStarted = gameDocument.UtcDateTimeStarted,
                    NextPlayer = gameDocument.NextPlayer == GameModel.Players.Player1 ? Players.Player1 : Players.Player2,
                    Player1State = new PlayerState
                    {
                        PlayerId = gameDocument.Player1.PlayerId,
                        Points = gameDocument.Player1.Points
                    },
                    Player2State = new PlayerState
                    {
                        PlayerId = gameDocument.Player2.PlayerId,
                        Points = gameDocument.Player2.Points
                    },
                    Winner = gameDocument.Winner == GameModel.Players.Player1 ? Players.Player1 : Players.Player2,
                    Status = gameDocument.Status == GameModel.GameStatus.Finished
                        ? GameStatus.Finished
                        : gameDocument.Status == GameModel.GameStatus.InProgress
                            ? GameStatus.InProgress
                            : GameStatus.Finished
                };
            }
        }
    }
}