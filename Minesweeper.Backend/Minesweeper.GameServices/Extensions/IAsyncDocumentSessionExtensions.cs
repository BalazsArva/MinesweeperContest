using System.Threading;
using System.Threading.Tasks;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.GameModel;
using Raven.Client.Documents.Session;

namespace Minesweeper.GameServices.Extensions
{
    public static class IAsyncDocumentSessionExtensions
    {
        public static Task<Game> LoadGameAsync(this IAsyncDocumentSession session, string gameId, CancellationToken cancellationToken)
        {
            var id = session.Advanced.DocumentStore.GetPrefixedDocumentId<Game>(gameId);

            return session.LoadAsync<Game>(id, cancellationToken);
        }
    }
}