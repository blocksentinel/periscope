using System;
using System.IO;
using System.Threading.Tasks;
using Cinder.Core;
using Cinder.Core.Exceptions;
using Cinder.Extensions;
using Cinder.Extensions.DependencyInjection;
using Cinder.Indexer.Host.Infrastructure.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Cinder.Indexer.Host.Infrastructure
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddEnvironmentVariables()
            .Build();

        public static async Task<int> Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration).CreateLogger();

            try
            {
                Log.Information("Starting Indexer; Version: {Version}; Build Date: {BuildDate}", VersionInfo.Version,
                    VersionInfo.BuildDate);
                IHost host = new HostBuilder().ConfigureServices((hostContext, services) =>
                    {
                        services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
                        services.AddHostedService<AddressIndexerService>();
                        services.AddHostedService<AddressRefresherService>();
                        services.AddHostedService<BlockIndexerService>();
                        services.Configure<Settings>(options => Configuration.Bind(options));
                        services.AddBlockchain();
                        services.AddDatabase();
                        services.AddEvents();
                    })
                    .UseSerilog()
                    .Build();

                await host.RunAsync().AnyContext();

                return 0;
            }
            catch (Exception e)
            {
                if (!(e is LoggedException))
                {
                    Log.Fatal(e, "Indexer terminated unexpectedly");
                }

                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
