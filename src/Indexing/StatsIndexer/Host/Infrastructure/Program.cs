using System.IO;
using System.Threading.Tasks;
using Cinder.Extensions;
using Cinder.Extensions.DependencyInjection;
using Cinder.Indexing.HostBase;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Hosting;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Jobs;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Services;
using Foundatio.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure
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
            HostWrapper hostWrapper = new HostWrapper
            {
                DefaultBuilder = new HostBuilder().ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
                    services.AddHostedService<StatsIndexerHost>();
                    services.Configure<SettingsBase>(options => Configuration.Bind(options));
                    services.Configure<IndexerSettings>(options => Configuration.Bind(options));
                    services.AddSingleton<NetInfoService>();
                    services.AddSingleton<IQueue<NetInfoWorkItem>>(sp => new InMemoryQueue<NetInfoWorkItem>());
                    services.AddSingleton<IConnectionMultiplexer>(sp =>
                    {
                        IOptions<IndexerSettings> options = sp.GetService<IOptions<IndexerSettings>>();

                        return ConnectionMultiplexer.Connect(options.Value.Redis.ConnectionString);
                    });
                    services.AddDatabase();
                    services.AddEvents();
                    services.AddWeb3();
                })
            };

            return await hostWrapper.Run("Stats Indexer", Configuration).AnyContext();
        }
    }
}
