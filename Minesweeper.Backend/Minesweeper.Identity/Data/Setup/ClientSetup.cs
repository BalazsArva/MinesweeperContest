using System.Collections.Generic;
using System.Linq;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.Models;
using Minesweeper.Common;
using Client = IdentityServer4.EntityFramework.Entities.Client;
using ClientCorsOrigin = IdentityServer4.EntityFramework.Entities.ClientCorsOrigin;
using ClientGrantType = IdentityServer4.EntityFramework.Entities.ClientGrantType;
using ClientRedirectUri = IdentityServer4.EntityFramework.Entities.ClientRedirectUri;
using ClientScope = IdentityServer4.EntityFramework.Entities.ClientScope;
using ClientSecret = IdentityServer4.EntityFramework.Entities.ClientSecret;

namespace Minesweeper.Identity.Data.Setup
{
    public static class ClientSetup
    {
        private static readonly List<Client> _knownClients = new List<Client>
        {
            new Client
            {
                Enabled = true,
                ClientId = "Minesweeper.API",
                RequireClientSecret = false,
                ClientSecrets = new List<ClientSecret>
                {
                    // TODO: Set up secret
                    // new IdentityServer4.EntityFramework.Entities.ClientSecret{ Value = "Secret".Sha256() }
                },
                ClientName = "Minesweeper game",
                Description = "The minesweeper game's browser application.",
                AllowedCorsOrigins = new List<ClientCorsOrigin>
                {
                    // TODO: This does not allow postman
                    // TODO: Make these configurable
                    new ClientCorsOrigin { Origin = "http://localhost:9000" }
                },

                // TODO: Refactor this
                AllowedGrantTypes = GrantTypes.Implicit.Concat(GrantTypes.ResourceOwnerPassword).Select(grantType => new ClientGrantType
                {
                    GrantType =  grantType
                }).ToList(),
                AllowedScopes = new List<ClientScope>
                {
                    // TODO: Move custom constants to a central location
                    new ClientScope { Scope = "Minesweeper.Apis.Game" },
                    new ClientScope { Scope = CustomScopes.CustomProfile },
                    new ClientScope { Scope = IdentityServerConstants.StandardScopes.OpenId },
                    new ClientScope { Scope = IdentityServerConstants.StandardScopes.Profile },
                    new ClientScope { Scope = IdentityServerConstants.StandardScopes.Email }
                },
                AllowAccessTokensViaBrowser = true,

                // TODO: Make these configurable
                ClientUri = "http://localhost:9000",
                RedirectUris = new List<ClientRedirectUri>()
                {
                    new ClientRedirectUri { RedirectUri = "http://localhost:9000" }
                }
            }
        };

        public static void EnsureClientsRegistered(ConfigurationDbContext configurationDbContext)
        {
            var clientsInDb = configurationDbContext.Clients;

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