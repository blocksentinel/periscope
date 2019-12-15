using System;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Exceptions;
using Cinder.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cinder.Indexer.Host.Infrastructure.Hosting
{
    public class BlockIndexerService : BackgroundService
    {
        private readonly IBlockIndexerRunner _blockIndexer;
        private readonly ILogger<BlockIndexerService> _logger;

        public BlockIndexerService(ILogger<BlockIndexerService> logger, IBlockIndexerRunner blockIndexer)
        {
            _logger = logger;
            _blockIndexer = blockIndexer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _blockIndexer.Run(stoppingToken).AnyContext();
                }
                catch (LoggedException) { }
                catch (Exception e)
                {
                    _logger.LogError(e, "Block indexer threw a non-logged exception");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).AnyContext();
            }
        }
    }
}
