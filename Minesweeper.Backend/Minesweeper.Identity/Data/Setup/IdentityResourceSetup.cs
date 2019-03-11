using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityResource = IdentityServer4.EntityFramework.Entities.IdentityResource;

namespace Minesweeper.Identity.Data.Setup
{
    public static class IdentityResourceSetup
    {
        // TODO: Delete if no longer needed
        private static List<IdentityResource> _knownIdentityResources = new List<IdentityResource>
        {
            new IdentityResource
            {
                Description = "Represents identity resources used by the minesweeper game.",
                DisplayName = "Minesweeper Identity Data",
                Name = "Minesweeper.Identity",
                Enabled = true,
                ShowInDiscoveryDocument = true
            }
        };

        public static void EnsureClientsRegistered(ConfigurationDbContext configurationDbContext)
        {
            var identityResourcesInDb = configurationDbContext.IdentityResources;

            foreach (var identityResource in _knownIdentityResources)
            {
                var identityResourceInDb = identityResourcesInDb.FirstOrDefault(r => r.Name == identityResource.Name);

                if (identityResourceInDb != null)
                {
                    identityResourcesInDb.Remove(identityResourceInDb);
                }

                identityResourcesInDb.Add(identityResource);
            }

            configurationDbContext.SaveChanges();

            _knownIdentityResources = null;
        }
    }
}