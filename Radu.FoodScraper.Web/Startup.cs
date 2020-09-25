using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Radu.FoodScraper.Data.Sql;
using Radu.FoodScraper.Web.Services;
using System.Linq;

namespace Radu.PureScraper.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddSingleton<ScraperIdentifierService>();
            services.AddSingleton<MapperService>();

            var connectionString = Configuration["Data:ConnectionString"];

            services.AddTransient(d => new DataService(connectionString, d.GetService<ILogger<DataService>>()));

            services.AddSwaggerGen(c =>
            {
                var assembly = this.GetType().Assembly;
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Version = "v1",
                    Title = assembly.GetName().Name,
                    Description = assembly.FullName
                }); ;
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "help/{documentName}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = "help";
                var endpoint = "/help/v1/swagger.json";
                c.SwaggerEndpoint(endpoint, "Radu.FoodScraper.Web");
            });
        }
    }
}
