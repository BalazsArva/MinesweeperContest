﻿using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using ApiResource = IdentityServer4.EntityFramework.Entities.ApiResource;
using ApiScope = IdentityServer4.EntityFramework.Entities.ApiScope;

namespace Minesweeper.Identity.Data.Setup
{
    public static class ApiResourceSetup
    {
        // TODO: Ensure this scope is actually used for gaming
        private static readonly List<ApiResource> _knownApiResources = new List<ApiResource>
        {
            new ApiResource
            {
                Description = "Represents the minesweeper game API.",
                DisplayName = "Minesweeper Game API",
                Scopes = new List<ApiScope>
                {
                    new ApiScope { Name = "Minesweeper.Apis.Game", Required = true, ShowInDiscoveryDocument = true }
                },
                Name = "Minesweeper",
                Enabled = true
            }
        };

        public static void EnsureApiResourcesRegistered(ConfigurationDbContext configurationDbContext)
        {
            var apiResourcesInDb = configurationDbContext.ApiResources;

            foreach (var apiResource in _knownApiResources)
            {
                var apiResourceInDb = apiResourcesInDb.FirstOrDefault(r => r.Name == apiResource.Name);

                if (apiResourceInDb != null)
                {
                    apiResourcesInDb.Remove(apiResourceInDb);
                }

                apiResourcesInDb.Add(apiResource);
            }

            configurationDbContext.SaveChanges();
        }
    }
}