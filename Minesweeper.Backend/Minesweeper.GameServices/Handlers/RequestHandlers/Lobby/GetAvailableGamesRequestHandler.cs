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
    public class GetAvailableGamesRequestHandler : IGetAvailableGamesRequestHandler
    {
        private readonly IDocumentStore _documentStore;

        public GetAvailableGamesRequestHandler(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<GetAvailableGamesResult> HandleAsync(GetAvailableGamesRequest request, CancellationToken cancellationToken)
        {
            // TODO: Create a validator for the parameters

            using (var session = _documentStore.OpenAsyncSession())
            {
                var results = await session
                    .Query<GameModel.Game>()
                    .Statistics(out var statistics)
                    .Where(g => g.InvitedPlayerId == null && g.Status == GameStatus.NotStarted && g.Player2.PlayerId == null && g.Player1.PlayerId != request.UserId)
                    .Paginate(request.Page, request.PageSize)
                    .Select(g => new
                    {
                        g.Id,
                        Player1Id = g.Player1.PlayerId,
                        Player1DisplayName = g.Player1.DisplayName,
                        g.Rows,
                        g.Columns,
                        g.Mines
                    })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var transformedResults = results
                    .Select(r => new AvailableGame
                    {
                        GameId = _documentStore.TrimCollectionPrefixFromDocumentId<GameModel.Game>(r.Id),
                        HostPlayerId = r.Player1Id,
                        HostPlayerDisplayName = r.Player1DisplayName,
                        Rows = r.Rows,
                        Columns = r.Columns,
                        Mines = r.Mines
                    })
                    .ToList();

                return new GetAvailableGamesResult
                {
                    AvailableGames = transformedResults,
                    Total = statistics.TotalResults
                };
            }
        }
    }
}