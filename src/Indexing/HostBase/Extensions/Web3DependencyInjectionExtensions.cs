using Cinder.Core.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nethereum.Web3;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Web3DependencyInjectionExtensions
    {
        public static void AddWeb3(this IServiceCollection services)
        {
            services.AddSingleton<IWeb3>(sp =>
            {
                IOptions<Settings> options = sp.GetService<IOptions<Settings>>();

                return new Web3(options.Value.Node.RpcUrl);
            });
        }
    }
}
