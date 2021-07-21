using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Jobs;
using Foundatio.Messaging;
using Foundatio.Queues;
using Microsoft.Extensions.Hosting;
using Periscope.Core.Events;
using Periscope.Core.Extensions;
using Periscope.Core.Jobs;

namespace Periscope.Jobs.Hosting
{
    public class StatsIndexerHost : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly CirculatingSupplyJob _circulatingSupplyJob;
        private readonly NetInfoJob _netInfoJob;
        private readonly PriceJob _priceJob;
        private readonly IQueue<NetInfoWorkItem> _queue;

        public StatsIndexerHost(IMessageBus bus, CirculatingSupplyJob circulatingSupplyJob, NetInfoJob netInfoJob,
            PriceJob priceJob, IQueue<NetInfoWorkItem> queue)
        {
            _bus = bus;
            _circulatingSupplyJob = circulatingSupplyJob;
            _netInfoJob = netInfoJob;
            _priceJob = priceJob;
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> tasks = new()
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
                _netInfoJob.RunContinuousAsync(cancellationToken: stoppingToken),
                _priceJob.RunContinuousAsync(TimeSpan.FromSeconds(60), cancellationToken: stoppingToken),
                _circulatingSupplyJob.RunContinuousAsync(TimeSpan.FromSeconds(60), cancellationToken: stoppingToken)
            };

            await Task.WhenAll(tasks).AnyContext();
        }
    }
}
