using Cinder.Indexers.HostBase;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Nethereum.Parity;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Web3
    {
        public static void AddWeb3(this IServiceCollection services)
        {
            services.AddSingleton<IWeb3Parity>(sp =>
            {
                IOptions<SettingsBase> options = sp.GetService<IOptions<SettingsBase>>();

                return new Web3Parity(options.Value.Node.RpcUrl);
            });
        }
    }
}
