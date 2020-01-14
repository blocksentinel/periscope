using Cinder.Indexing.AddressIndexer.Host.Infrastructure.Jobs;
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
