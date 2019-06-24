using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.Identity.Data;
using Minesweeper.Identity.Data.Entities;
using Minesweeper.Identity.IdentityServices;

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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(cors => cors.AddPolicy("Frontend", configure => configure.SetIsOriginAllowed(url => url == "http://localhost:9000").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

            services
                .AddDbContext<MinesweeperIdentityDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Minesweeper.IdentityDb")));

            services
                .AddIdentity<AppUser, IdentityRole>()
                .AddEntityFrameworkStores<MinesweeperIdentityDbContext>()
                .AddUserManager<AspNetUserManager<AppUser>>()
                .AddRoleManager<AspNetRoleManager<IdentityRole>>()
                .AddClaimsPrincipalFactory<CustomClaimsPrincipalFactory>()
                .AddDefaultTokenProviders();

            services
                .AddIdentityServer()
                // TODO: Replace with a proper signing credential later
                .AddDeveloperSigningCredential()
                .AddConfigurationStore(options =>
                {
                    // This adds the config data from DB (clients, resources)
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(Configuration.GetConnectionString("Minesweeper.ConfigDb"), sql => sql.MigrationsAssembly(migrationsAssembly));
                })
                .AddOperationalStore(options =>
                {
                    // This adds the operational data from DB (codes, tokens, consents)
                    options.ConfigureDbContext = b =>
                        b.UseSqlServer(Configuration.GetConnectionString("Minesweeper.OperationalDb"), sql => sql.MigrationsAssembly(migrationsAssembly));

                    // This enables automatic token cleanup. This is optional.
                    options.EnableTokenCleanup = true;
                })
                .AddAspNetIdentity<AppUser>()
                .AddProfileService<CustomProfileService>();
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

            DatabaseSeeder.SeedDatabase(app);
        }
    }
}