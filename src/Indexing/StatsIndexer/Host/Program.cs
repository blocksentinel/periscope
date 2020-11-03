using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Periscope.Core.Extensions;
using Periscope.Indexing.HostBase;
using Periscope.Indexing.HostBase.Extensions;
using Periscope.Indexing.StatsIndexer.Host.Infrastructure.Extensions;
using Periscope.Indexing.StatsIndexer.Host.Infrastructure.Hosting;
using Periscope.Stats.Extensions;

namespace Periscope.Indexing.StatsIndexer.Host
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
