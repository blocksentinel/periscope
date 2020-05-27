using Cinder.Core.SharedKernel;
using Cinder.Indexing.AddressIndexer.Host.Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class CustomOptions
    {
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AddressRefresherSettings>(configuration.GetSection("AddressRefresher").Bind);
        }
    }
}
