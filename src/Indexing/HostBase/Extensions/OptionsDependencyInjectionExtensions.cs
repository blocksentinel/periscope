using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Periscope.Core.SharedKernel;

namespace Periscope.Indexing.HostBase.Extensions
{
    public static class OptionsDependencyInjectionExtensions
    {
        public static void AddOptions(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<Settings>(configuration.Bind);
        }
    }
}
