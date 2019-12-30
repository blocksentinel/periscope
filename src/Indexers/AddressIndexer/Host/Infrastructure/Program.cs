using System.IO;
using System.Threading.Tasks;
using Cinder.Extensions;
using Cinder.Extensions.DependencyInjection;
using Cinder.Indexers.AddressIndexer.Host.Infrastructure.Hosting;
using Cinder.Indexers.HostBase;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Cinder.Indexers.AddressIndexer.Host.Infrastructure
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
                    services.AddHostedService<AddressIndexerService>();
                    services.AddHostedService<AddressRefresherService>();
                    services.Configure<SettingsBase>(options => Configuration.Bind(options));
                    services.AddDatabase();
                    services.AddEvents();
                    services.AddWeb3();
                })
            };

            return await hostWrapper.Run("Address Indexer", Configuration).AnyContext();
        }
    }
}
