using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Events;
using Cinder.Extensions;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko;
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
        private readonly NetInfoJob _netStatsJob;
        private readonly PriceJob _priceJob;
        private readonly IQueue<NetInfoWorkItem> _queue;

        public StatsIndexerHost(ILoggerFactory loggerFactory, IMessageBus bus, NetInfoService netInfoService,
            IConnectionMultiplexer muxer, IQueue<NetInfoWorkItem> queue, ICoinGeckoApi api)
        {
            _bus = bus;
            _queue = queue;
            _netStatsJob = new NetInfoJob(_queue, loggerFactory, netInfoService, muxer);
            _priceJob = new PriceJob(loggerFactory, muxer, api);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> tasks = new List<Task>
            {
                _bus.SubscribeAsync<BlockFoundEvent>(
                    async @event =>
                    {
                        await _queue.EnqueueAsync(new NetInfoWorkItem
                            {
                                BlockNumber = @event.BlockNumber,
                                Difficulty = @event.Difficulty,
                                UncleCount = @event.UncleCount,
                                TransactionCount = @event.TransactionCount,
                                Timestamp = @event.Timestamp
                            })
                            .AnyContext();
                    }, stoppingToken),
                _netStatsJob.RunContinuousAsync(cancellationToken: stoppingToken),
                _priceJob.RunContinuousAsync(TimeSpan.FromSeconds(60), cancellationToken: stoppingToken)
            };

            await Task.WhenAll(tasks).AnyContext();
        }

        public override void Dispose()
        {
            _netStatsJob?.Dispose();
            _priceJob?.Dispose();
            base.Dispose();
        }
    }
}
