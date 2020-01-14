using System.IO;
using System.Threading.Tasks;
using Cinder.Extensions;
using Cinder.Extensions.DependencyInjection;
using Cinder.Indexing.BlockIndexer.Host.Infrastructure.Hosting;
using Cinder.Indexing.HostBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cinder.Indexing.BlockIndexer.Host.Infrastructure
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
                    services.AddHostedService<BlockIndexerHost>();
                    services.AddOptions(Configuration);
                    services.AddBlockchain();
                    services.AddDatabase();
                    services.AddEvents();
                    services.AddWeb3();
                }))
                .Run("Block Indexer", Configuration)
                .AnyContext();
        }
    }
}
