using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Extensions;
using Minesweeper.WebAPI.Hubs;

namespace Minesweeper.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddRavenDb(Configuration);
            services.AddGameServices();
            services.AddSignalR();
            services.AddCors(cors => cors.AddPolicy("Frontend", configure => configure.SetIsOriginAllowed(url => url == "http://localhost:9000").AllowAnyHeader().AllowAnyMethod().AllowCredentials()));
            services.AddMediatR();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("Frontend");

            //app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()/*.AllowCredentials()*/);
            app.UseHttpsRedirection();

            app.UseSignalR(routes =>
            {
                routes.MapHub<GameHub>("/hubs/game");
            });
            app.UseMvc();
        }
    }
}