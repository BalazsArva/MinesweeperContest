using Raven.Client.Documents.Linq;

namespace Minesweeper.DataAccess.RavenDb.Extensions
{
    public static class IAsyncDocumentSessionExtensions
    {
        public static IRavenQueryable<T> Paginate<T>(this IRavenQueryable<T> queryable, int page, int pageSize)
        {
            return queryable
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
    }
}