using Foundatio.Queues;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Periscope.Indexing.AddressIndexer.Host.Infrastructure.Jobs;
using StackExchange.Redis;

namespace Periscope.Indexing.AddressIndexer.Host.Infrastructure.Extensions
{
    public static class JobDependencyInjectionExtensions
    {
        public static void AddJobs(this IServiceCollection services)
        {
            services.AddSingleton<IQueue<AddressTransactedWorkItem>>(sp =>
            {
                IConnectionMultiplexer muxer = sp.GetService<IConnectionMultiplexer>();
                ILoggerFactory loggerFactory = sp.GetService<ILoggerFactory>();

                return new RedisQueue<AddressTransactedWorkItem>(builder =>
                    builder.ConnectionMultiplexer(muxer).LoggerFactory(loggerFactory));
            });
            services.AddSingleton<AddressRefresherJob>();
            services.AddSingleton<AddressTransactedJob>();
        }
    }
}
