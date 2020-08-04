using System;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko;
using Microsoft.Extensions.DependencyInjection;
using Refit;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
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
