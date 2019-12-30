using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Documents;
using Cinder.Events;
using Cinder.Extensions;
using Foundatio.Messaging;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cinder.Indexers.AddressIndexer.Host.Infrastructure.Hosting
{
    public class AddressIndexerService : BackgroundService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMessageBus _bus;
        private readonly ILogger<AddressIndexerService> _logger;
        private readonly ConcurrentDictionary<string, CinderAddress> _queue = new ConcurrentDictionary<string, CinderAddress>();

        public AddressIndexerService(ILogger<AddressIndexerService> logger, IMessageBus bus, IAddressRepository addressRepository)
        {
            _logger = logger;
            _bus = bus;
            _addressRepository = addressRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _bus.SubscribeAsync<AddressesTransactedEvent>(AddressesTransactedEventHandler, stoppingToken).AnyContext();

            while (!stoppingToken.IsCancellationRequested)
            {
                List<CinderAddress> addresses = new List<CinderAddress>();
                int i = 0;
                foreach (KeyValuePair<string, CinderAddress> entry in _queue)
                {
                    if (i > 50)
                    {
                        break;
                    }

                    if (!_queue.TryRemove(entry.Key, out CinderAddress address))
                    {
                        continue;
                    }

                    addresses.Add(address);
                    i++;
                }

                if (addresses.Any())
                {
                    await _addressRepository.BulkInsertAddressesIfNotExists(addresses, stoppingToken).AnyContext();
                }

                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken).AnyContext();
            }
        }

        private Task AddressesTransactedEventHandler(AddressesTransactedEvent arg)
        {
            foreach (string address in arg.Addresses)
            {
                _queue.TryAdd(address, new CinderAddress {Hash = address, ForceRefresh = true});
            }

            return Task.CompletedTask;
        }
    }
}
