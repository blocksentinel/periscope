using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Periscope.Core.Settings;

namespace Periscope.Jobs.Extensions
{
    public static class CustomOptionsDependencyInjectionExtensions
    {
        public static void AddCustomOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AddressRefresherSettings>(configuration.GetSection("AddressRefresher").Bind);
        }
    }
}
