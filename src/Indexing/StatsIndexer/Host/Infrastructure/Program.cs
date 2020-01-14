using System.IO;
using System.Threading.Tasks;
using Cinder.Extensions;
using Cinder.Extensions.DependencyInjection;
using Cinder.Indexing.HostBase;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
            return await HostWrapper.Create(builder => builder.ConfigureServices((hostContext, services) =>
                {
                    services.Configure<ConsoleLifetimeOptions>(options => options.SuppressStatusMessages = true);
                    services.AddHostedService<StatsIndexerHost>();
                    services.AddOptions(Configuration);
                    services.AddClients();
                    services.AddDatabase();
                    services.AddEvents();
                    services.AddJobs();
                    services.AddStats();
                    services.AddWeb3();
                }))
                .Run("Stats Indexer", Configuration)
                .AnyContext();
        }
    }
}
