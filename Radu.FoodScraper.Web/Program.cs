using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Radu.PureScraper.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var logger = new LoggerConfiguration()
                               .MinimumLevel.Verbose()
                               .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                               .MinimumLevel.Override("System", LogEventLevel.Warning)
                               .MinimumLevel.Override("Microsoft.AspNetCore.Authentication", LogEventLevel.Verbose)
                               .Enrich.FromLogContext()
                               .WriteTo.RollingFile("Logs/{Date}.txt", outputTemplate: "{Timestamp:HH:mm:ss} [{Level}] [{SourceContext}] {Message}{NewLine}{Exception}")
                               .CreateLogger();

            var host = Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddSerilog(logger);
                });

            return host;
        }
    }
}
