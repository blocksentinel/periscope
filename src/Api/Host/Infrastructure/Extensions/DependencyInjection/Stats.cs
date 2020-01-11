using Cinder.Api.Host.Infrastructure;
using Cinder.Stats;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Stats
    {
        public static void AddStats(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                IOptions<Settings> options = sp.GetService<IOptions<Settings>>();

                return ConnectionMultiplexer.Connect(options.Value.Redis.ConnectionString);
            });
            services.AddSingleton<IStatsCache>(sp =>
            {
                IConnectionMultiplexer muxer = sp.GetService<IConnectionMultiplexer>();
                ILoggerFactory loggerFactory = sp.GetService<ILoggerFactory>();

                return new StatsCache(new RedisCacheClientOptions {ConnectionMultiplexer = muxer, LoggerFactory = loggerFactory});
            });
        }
    }
}
