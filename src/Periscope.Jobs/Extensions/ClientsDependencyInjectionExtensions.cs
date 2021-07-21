using System;
using Microsoft.Extensions.DependencyInjection;
using Periscope.Core.Clients.CoinGecko;
using Refit;

namespace Periscope.Jobs.Extensions
{
    public static class ClientDependencyInjectionExtensions
    {
        public static void AddClients(this IServiceCollection services)
        {
            services.AddRefitClient<ICoinGeckoApi>()
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.coingecko.com/api"));
        }
    }
}
