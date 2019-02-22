using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minesweeper.DataAccess.RavenDb.Extensions;
using Minesweeper.GameServices.Extensions;

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
            services.AddCors(/*cors =>
            {
                cors.AddPolicy("FrontendPolicy", new Microsoft.AspNetCore.Cors.Infrastructure.CorsPolicy
                {
                    Origins = { "http://localhost:9000" },
                    IsOriginAllowed = true
                });
            }*/);
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

            app.UseCors(builder =>
            {
                builder.SetIsOriginAllowed(_ => true);
            });
            app.UseHttpsRedirection();
            app.UseMvc();
        }
    }
}