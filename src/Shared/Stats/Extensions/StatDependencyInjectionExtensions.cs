using Cinder.Core.SharedKernel;
using Foundatio.Caching;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class StatDependencyInjectionExtensions
    {
        public static void AddStats(this IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                IOptions<Settings> options = sp.GetService<IOptions<Settings>>();

                return ConnectionMultiplexer.Connect(options.Value.Redis.ConnectionString);
            });
            services.AddSingleton<IHybridCacheClient>(sp =>
            {
                IConnectionMultiplexer muxer = sp.GetService<IConnectionMultiplexer>();
                ILoggerFactory loggerFactory = sp.GetService<ILoggerFactory>();

                return new RedisHybridCacheClient(new RedisHybridCacheClientOptions
                {
                    ConnectionMultiplexer = muxer, LoggerFactory = loggerFactory
                });
            });
        }
    }
}
