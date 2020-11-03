using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Periscope.Indexing.AddressIndexer.Host.Infrastructure.Settings;

namespace Periscope.Indexing.AddressIndexer.Host.Infrastructure.Extensions
{
    public static class CustomOptionsDependencyInjectionExtensions
    {
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AddressRefresherSettings>(configuration.GetSection("AddressRefresher").Bind);
        }
    }
}
