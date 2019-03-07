using System.Reflection;
using IdentityServer4.EntityFramework.DbContexts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Identity.Data;
using Minesweeper.Identity.Data.Entities;
using Minesweeper.Identity.Data.Setup;

namespace Minesweeper.Identity
{
    public class Startup
    {
        private static readonly string migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<MinesweeperIdentityDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("Minesweeper.IdentityDb")));

            services.AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MinesweeperIdentityDbContext>()
                .AddUserManager<AspNetUserManager<AppUser>>()
                .AddDefaultTokenProviders();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddCors(cors => cors.AddPolicy("Frontend", configure => configure.SetIsOriginAllowed(url => url == "http://localhost:9000").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            services.AddIdentityServer()
                // TODO: Replace with a proper signing credential later
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    // this adds the config data from DB (clients, resources)
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(Configuration.GetConnectionString("Minesweeper.ConfigDb"), sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    // this adds the operational data from DB (codes, tokens, consents)
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(Configuration.GetConnectionString("Minesweeper.OperationalDb"), sql => sql.MigrationsAssembly(migrationsAssembly));

                    // this enables automatic token cleanup. this is optional.
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<AppUser>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseCors("Frontend");

            app.UseIdentityServer();

            app.UseHttpsRedirection();
            app.UseMvc();

            SeedDatabase(app);
        }

        private void SeedDatabase(IApplicationBuilder app)
        {
            // TODO: Only migrate if needed
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                var identityServerConfigurationDbContext = serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();

                ClientSetup.EnsureClientsRegistered(identityServerConfigurationDbContext);
            }
        }
    }
}