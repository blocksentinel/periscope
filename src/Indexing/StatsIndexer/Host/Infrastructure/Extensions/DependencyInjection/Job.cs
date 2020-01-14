using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Jobs;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Services;
using Foundatio.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Job
    {
        public static void AddJobs(this IServiceCollection services)
        {
            services.AddSingleton<INetInfoService, NetInfoService>();
            services.AddSingleton<IQueue<NetInfoWorkItem>>(sp =>
            {
                IConnectionMultiplexer muxer = sp.GetService<IConnectionMultiplexer>();
                ILoggerFactory loggerFactory = sp.GetService<ILoggerFactory>();

                return new RedisQueue<NetInfoWorkItem>(builder =>
                    builder.ConnectionMultiplexer(muxer).LoggerFactory(loggerFactory));
            });
            services.AddSingleton<NetInfoJob>();
            services.AddSingleton<PriceJob>();
            services.AddSingleton<CirculatingSupplyJob>();
        }
    }
}
