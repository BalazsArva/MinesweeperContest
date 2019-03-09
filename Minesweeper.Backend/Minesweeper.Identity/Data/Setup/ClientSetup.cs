using System.Collections.Generic;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using ClientScope = IdentityServer4.EntityFramework.Entities.ClientScope;

namespace Minesweeper.Identity.Data.Setup
{
    public static class ClientSetup
    {
        private static readonly List<Client> _knownClients = new List<Client>
        {
            new Client
            {
                Enabled = true,
                ClientId = "Minesweeper.Frontend",
                RequireClientSecret = false,
                ClientName = "Frontend application",
                Description = "The minesweeper game's browser application.",
                AllowedCorsOrigins = new List<IdentityServer4.EntityFramework.Entities.ClientCorsOrigin>
                {
                    // TODO: This does not allow postman
                    new IdentityServer4.EntityFramework.Entities.ClientCorsOrigin { Origin = "http://localhost:9000"}
                },

                // TODO: Refactor this
                AllowedGrantTypes = GrantTypes.Implicit.Concat(GrantTypes.ResourceOwnerPassword).Select(grantType => new IdentityServer4.EntityFramework.Entities.ClientGrantType
                {
                    GrantType =  grantType
                }).ToList(),
                AllowedScopes = new List<ClientScope>
                {
                    new ClientScope { Scope = "Game" }
                },
                AllowAccessTokensViaBrowser = true,

                // TODO: Make these configurable
                ClientUri = "http://localhost:9000",
                RedirectUris = new List<IdentityServer4.EntityFramework.Entities.ClientRedirectUri>()
                {
                    new IdentityServer4.EntityFramework.Entities.ClientRedirectUri { RedirectUri = "http://localhost:9000" }
                }
                //AllowedCorsOrigins =
            }
        };

        public static void EnsureClientsRegistered(ConfigurationDbContext configurationDbContext)
        {
            var clientsInDb = configurationDbContext.Clients;

            // TODO: Review this
            var gameApiResource = configurationDbContext.ApiResources.FirstOrDefault(x => x.Name == "Game");

            if (gameApiResource != null)
            {
                configurationDbContext.ApiResources.Remove(gameApiResource);
            }

            configurationDbContext.ApiResources.Add(new IdentityServer4.EntityFramework.Entities.ApiResource
            {
                Description = "Represents the minesweeper game.",
                DisplayName = "Minesweeper game API",
                Scopes = new List<IdentityServer4.EntityFramework.Entities.ApiScope>
                {
                    new IdentityServer4.EntityFramework.Entities.ApiScope{ DisplayName ="Game API", Name = "Game", Required=true, ShowInDiscoveryDocument=true}
                },
                Name = "Game",
                Enabled = true
            });

            foreach (var client in _knownClients)
            {
                var clientInDb = clientsInDb.FirstOrDefault(c => c.ClientId == client.ClientId);

                if (clientInDb != null)
                {
                    clientsInDb.Remove(clientInDb);
                }

                clientsInDb.Add(client);
            }

            configurationDbContext.SaveChanges();
        }
    }
}