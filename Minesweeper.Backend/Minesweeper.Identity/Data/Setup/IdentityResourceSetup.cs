using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Minesweeper.Common;

namespace Minesweeper.Identity.Data.Setup
{
    public static class IdentityResourceSetup
    {
        private static readonly List<IdentityResource> _knownIdentityResources = new List<IdentityResource>
        {
            new IdentityResources.Email(),
            new IdentityResources.Profile(),
            new IdentityResources.OpenId(),
            new IdentityResource
            {
                DisplayName = "Custom profile",
                Description = "Contains profile information beyond what is provided by the OpenID spec.",
                Name = "CustomProfile",
                UserClaims = new List<string>
                {
                    CustomClaimTypes.DisplayName
                }
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

                identityResourcesInDb.Add(identityResource.ToEntity());
            }

            configurationDbContext.SaveChanges();
        }
    }
}