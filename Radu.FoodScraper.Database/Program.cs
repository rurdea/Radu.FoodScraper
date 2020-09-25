using DbUp;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;

namespace Radu.FoodScraper.Database
{
    class Program
    {
        static void Main(string[] args)
        {
            var configBuilder = new ConfigurationBuilder()
          .SetBasePath(Path.Combine(AppContext.BaseDirectory))
              .AddJsonFile("appsettings.json", optional: true);

            var config = configBuilder.Build();
            
            var builder =
                 DeployChanges.To
                     .SqlDatabase(config["ConnectionString"])
                     .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
                     .WithTransactionPerScript();

            builder
                .Configure(c =>
                {
                    c.ScriptExecutor.ExecutionTimeoutSeconds = Convert.ToInt32(config["ExecutionTimeoutInSeconds"]);
                    Console.WriteLine("ExecutionTimeoutSeconds = " + c.ScriptExecutor.ExecutionTimeoutSeconds);
                });

            var engine = builder
                .LogToConsole()
                .Build();

            var result = engine.PerformUpgrade();

            if (!result.Successful)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.Error);
                Console.ResetColor();
#if DEBUG
                Console.ReadLine();
#endif
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Success!");
            Console.ResetColor();
        }
    }
}
