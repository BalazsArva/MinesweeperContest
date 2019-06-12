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

        public static Task<PlayerMarks> LoadPlayerMarksAsync(this IAsyncDocumentSession session, string gameId, string playerId, CancellationToken cancellationToken)
        {
            var documentId = GetPlayerMarksDocumentId(session, gameId, playerId);

            return session.LoadAsync<PlayerMarks>(documentId, cancellationToken);
        }

        public static string GetPlayerMarksDocumentId(this IAsyncDocumentSession session, string gameId, string playerId)
        {
            var collectionName = session.Advanced.DocumentStore.GetCollectionPrefixForDocument<PlayerMarks>(false);
            var separator = session.Advanced.DocumentStore.Conventions.IdentityPartsSeparator;

            return string.Join(separator, collectionName, gameId, playerId);
        }
    }
}