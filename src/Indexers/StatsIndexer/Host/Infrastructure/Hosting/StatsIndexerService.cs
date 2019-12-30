using System;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Events;
using Cinder.Extensions;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cinder.Indexers.StatsIndexer.Host.Infrastructure.Hosting
{
    public class StatsIndexerService : BackgroundService
    {
        private readonly IMessageBus _bus;
        private readonly ILogger<StatsIndexerService> _logger;

        public StatsIndexerService(ILogger<StatsIndexerService> logger, IMessageBus bus)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.SubscribeAsync<BlockFoundEvent>(BlockFoundEventHandler, stoppingToken).AnyContext();

            while (!stoppingToken.IsCancellationRequested) await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken).AnyContext();
        }

        private async Task BlockFoundEventHandler(BlockFoundEvent arg)
        {
            _logger.LogDebug("StatsIndexerService -> BlockFoundEventHandler Fired, Block: {Number}", arg.Number);
        }
    }
}
