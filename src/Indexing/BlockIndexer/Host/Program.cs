using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Periscope.Core.Extensions;
using Periscope.Indexing.BlockIndexer.Host.Infrastructure.Extensions;
using Periscope.Indexing.BlockIndexer.Host.Infrastructure.Hosting;
using Periscope.Indexing.HostBase;
using Periscope.Indexing.HostBase.Extensions;

namespace Periscope.Indexing.BlockIndexer.Host
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
