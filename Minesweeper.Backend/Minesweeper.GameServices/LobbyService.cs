using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Contracts;
using Minesweeper.GameServices.GameModel;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;

namespace Minesweeper.GameServices
{
    public class LobbyService : ILobbyService
    {
        private readonly IDocumentStore _documentStore;

        public LobbyService(IDocumentStore documentStore)
        {
            _documentStore = documentStore ?? throw new ArgumentNullException(nameof(documentStore));
        }

        public async Task<GetAvailableGamesResult> GetAvailableGamesAsync(string userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            // TODO: Create a validator for the parameters

            using (var session = _documentStore.OpenAsyncSession())
            {
                var results = await session
                    .Query<Game>()
                    .Statistics(out var statistics)
                    .Where(g => g.InvitedPlayerId == null && g.Status == GameStatus.NotStarted && g.Player2 == null && g.Player1.PlayerId != userId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(g => new { g.Id, Player1Id = g.Player1.PlayerId, Player1DisplayName = g.Player1.DisplayName, g.Rows, g.Columns, g.Mines })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var transformedResults = results
                    .Select(r => new AvailableGame(_documentStore.TrimCollectionPrefixFromDocumentId<Game>(r.Id), r.Player1Id, r.Player1DisplayName, r.Rows, r.Columns, r.Mines))
                    .ToList();

                return new GetAvailableGamesResult(transformedResults, statistics.TotalResults);
            }
        }

        public async Task<GetPlayersActiveGamesResult> GetPlayersActiveGamesAsync(string userId, int page, int pageSize, CancellationToken cancellationToken)
        {
            // TODO: Create a validator for the parameters

            using (var session = _documentStore.OpenAsyncSession())
            {
                var results = await session
                    .Query<Game>()
                    .Statistics(out var statistics)
                    .Where(g => g.Status == GameStatus.NotStarted || g.Status == GameStatus.InProgress)
                    .Where(g => g.Player1.PlayerId == userId || g.Player2.PlayerId == userId)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
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
                        var userIsPlayer1 = r.Player1Id == userId;

                        var otherPlayerId = userIsPlayer1 ? r.Player2Id : r.Player1Id;
                        var otherPlayerDisplayName = userIsPlayer1 ? r.Player2DisplayName : r.Player1DisplayName;

                        return new PlayersGame(_documentStore.TrimCollectionPrefixFromDocumentId<Game>(r.Id), otherPlayerId, otherPlayerDisplayName, r.Rows, r.Columns, r.Mines);
                    })
                    .ToList();

                return new GetPlayersActiveGamesResult(transformedResults, statistics.TotalResults);
            }
        }
    }
}