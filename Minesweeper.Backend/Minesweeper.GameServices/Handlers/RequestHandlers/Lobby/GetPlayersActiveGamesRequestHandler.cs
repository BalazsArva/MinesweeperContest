using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Contracts.Requests.Lobby;
using Minesweeper.GameServices.Contracts.Responses.Lobby;
using Minesweeper.GameServices.GameModel;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace Minesweeper.GameServices.Handlers.RequestHandlers.Lobby
{
    public class GetPlayersActiveGamesRequestHandler : IGetPlayersActiveGamesRequestHandler
    {
        private readonly IDocumentStore _documentStore;

        public GetPlayersActiveGamesRequestHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<GetPlayersActiveGamesResult> HandleAsync(GetPlayersActiveGamesRequest request, CancellationToken cancellationToken)
        {
            // TODO: Create a validator for the parameters

            using (var session = _documentStore.OpenAsyncSession())
            {
                var results = await session
                    .Query<GameModel.Game>()
                    .Statistics(out var statistics)
                    .Where(g => g.Status == GameStatus.NotStarted || g.Status == GameStatus.InProgress)
                    .Where(g => g.Player1.PlayerId == request.UserId || g.Player2.PlayerId == request.UserId)
                    .Paginate(request.Page, request.PageSize)
                    .Select(g => new
                    {
                        g.Id,
                        Player1Id = g.Player1.PlayerId,
                        Player1DisplayName = g.Player1.DisplayName,
                        Player2Id = g.Player2.PlayerId,
                        Player2DisplayName = g.Player2.DisplayName,
                        g.Rows,
                        g.Columns,
                        g.Mines
                    })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var transformedResults = results
                    .Select(r =>
                    {
                        var userIsPlayer1 = r.Player1Id == request.UserId;

                        var otherPlayerId = userIsPlayer1 ? r.Player2Id : r.Player1Id;
                        var otherPlayerDisplayName = userIsPlayer1 ? r.Player2DisplayName : r.Player1DisplayName;

                        return new PlayersGame
                        {
                            GameId = _documentStore.TrimCollectionPrefixFromDocumentId<GameModel.Game>(r.Id),
                            OtherPlayerId = otherPlayerId,
                            OtherPlayerDisplayName = otherPlayerDisplayName,
                            Rows = r.Rows,
                            Columns = r.Columns,
                            Mines = r.Mines
                        };
                    })
                    .ToList();

                return new GetPlayersActiveGamesResult
                {
                    PlayersGames = transformedResults,
                    Total = statistics.TotalResults
                };
            }
        }
    }
}