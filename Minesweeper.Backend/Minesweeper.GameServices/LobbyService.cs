﻿using System;
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
                    .Select(g => new { g.Id, Player1Id = g.Player1.PlayerId, Player1DisplayName = g.Player1.DisplayName, g.GameTable.Rows, g.GameTable.Columns, g.GameTable.Mines })
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var transformedResults = results
                    .Select(r => new AvailableGame(_documentStore.TrimCollectionPrefixFromDocumentId<Game>(r.Id), r.Player1Id, r.Player1DisplayName, r.Rows, r.Columns, r.Mines))
                    .ToList();

                return new GetAvailableGamesResult(transformedResults, statistics.TotalResults);
            }
        }
    }
}