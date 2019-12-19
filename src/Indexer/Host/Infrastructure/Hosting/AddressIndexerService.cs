using System;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Exceptions;
using Cinder.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cinder.Indexer.Host.Infrastructure.Hosting
{
    public class AddressIndexerService : BackgroundService
    {
        private readonly IAddressIndexerRunner _addressIndexer;
        private readonly ILogger<AddressIndexerService> _logger;

        public AddressIndexerService(ILogger<AddressIndexerService> logger, IAddressIndexerRunner addressIndexer)
        {
            _logger = logger;
            _addressIndexer = addressIndexer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _addressIndexer.Run(stoppingToken).AnyContext();
                }
                catch (LoggedException) { }
                catch (Exception e)
                {
                    _logger.LogError(e, "Address indexer threw a non-logged exception");
                }

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken).AnyContext();
            }
        }
    }
}
