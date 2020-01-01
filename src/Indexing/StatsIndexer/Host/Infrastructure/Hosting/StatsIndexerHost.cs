using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Events;
using Cinder.Extensions;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Jobs;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Services;
using Foundatio.Jobs;
using Foundatio.Messaging;
using Foundatio.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure.Hosting
{
    public class StatsIndexerHost : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly NetStatsJob _netStatsJob;
        private readonly IQueue<NetStatsWorkItem> _queue;

        public StatsIndexerHost(ILoggerFactory loggerFactory, IMessageBus bus, NetInfoService netInfoService,
            IConnectionMultiplexer muxer, IQueue<NetStatsWorkItem> queue)
        {
            _bus = bus;
            _queue = queue;
            _netStatsJob = new NetStatsJob(_queue, loggerFactory, netInfoService, muxer);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> tasks = new List<Task>
            {
                _bus.SubscribeAsync<BlockFoundEvent>(
                    async @event =>
                    {
                        await _queue.EnqueueAsync(new NetStatsWorkItem
                            {
                                BlockNumber = @event.BlockNumber,
                                Difficulty = @event.Difficulty,
                                UncleCount = @event.UncleCount,
                                TransactionCount = @event.TransactionCount,
                                Timestamp = @event.Timestamp
                            })
                            .AnyContext();
                    }, stoppingToken),
                _netStatsJob.RunContinuousAsync(cancellationToken: stoppingToken)
            };

            await Task.WhenAll(tasks).AnyContext();
        }

        public override void Dispose()
        {
            _netStatsJob?.Dispose();
            base.Dispose();
        }
    }
}
