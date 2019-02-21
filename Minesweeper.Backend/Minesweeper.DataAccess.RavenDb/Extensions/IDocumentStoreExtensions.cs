using Raven.Client.Documents;

namespace Minesweeper.DataAccess.RavenDb.Extensions
{
    public static class IDocumentStoreExtensions
    {
        public static string GetPrefixedDocumentId<TDocument>(this IDocumentStore documentStore, string entityId)
        {
            var prefix = documentStore.Conventions.GetCollectionName(typeof(TDocument));
            var separator = documentStore.Conventions.IdentityPartsSeparator;
            var prefixWithSeparator = $"{prefix}{separator}";

            if (entityId.StartsWith(prefixWithSeparator))
            {
                return entityId;
            }

            return $"{prefixWithSeparator}{entityId}";
        }

        public static string GetCollectionPrefixForDocument<TDocument>(this IDocumentStore documentStore, bool includeIdentityPartsSeparator = true)
        {
            var prefix = documentStore.Conventions.GetCollectionName(typeof(TDocument));

            if (includeIdentityPartsSeparator)
            {
                var separator = documentStore.Conventions.IdentityPartsSeparator;

                return $"{prefix}{separator}";
            }

            return prefix;
        }
    }
}