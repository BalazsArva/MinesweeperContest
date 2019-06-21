using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Identity.Data.Setup;

namespace Minesweeper.Identity.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedDatabase(IApplicationBuilder app)
        {
            // TODO: Only seed if needed
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var identityServerConfigurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                ClientSetup.EnsureClientsRegistered(identityServerConfigurationDbContext);
                ApiResourceSetup.EnsureApiResourcesRegistered(identityServerConfigurationDbContext);
                IdentityResourceSetup.EnsureClientsRegistered(identityServerConfigurationDbContext);
            }
        }
    }
}