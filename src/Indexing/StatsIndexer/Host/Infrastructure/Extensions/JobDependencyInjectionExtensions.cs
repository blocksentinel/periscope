using Foundatio.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Periscope.Indexing.StatsIndexer.Host.Infrastructure.Jobs;
using Periscope.Indexing.StatsIndexer.Host.Infrastructure.Services;
using StackExchange.Redis;

namespace Periscope.Indexing.StatsIndexer.Host.Infrastructure.Extensions
{
    public static class JobDependencyInjectionExtensions
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
