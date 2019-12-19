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
using Microsoft.Extensions.Logging;

namespace Cinder.Indexer.Host.Infrastructure
{
    public class AddressIndexerRunner : IAddressIndexerRunner
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IMessageBus _bus;
        private readonly ILogger<AddressIndexerRunner> _logger;
        private readonly ConcurrentDictionary<string, CinderAddress> _queue = new ConcurrentDictionary<string, CinderAddress>();

        public AddressIndexerRunner(ILogger<AddressIndexerRunner> logger, IMessageBus bus, IAddressRepository addressRepository)
        {
            _logger = logger;
            _bus = bus;
            _addressRepository = addressRepository;
        }

        public async Task Run(CancellationToken cancellationToken = default)
        {
            await _bus.SubscribeAsync<AddressesTransactedEvent>(AddressesTransactedEventHandler, cancellationToken).AnyContext();

            while (!cancellationToken.IsCancellationRequested)
            {
                List<CinderAddress> addresses = new List<CinderAddress>();
                int i = 0;
                foreach (KeyValuePair<string, CinderAddress> entry in _queue)
                {
                    if (i > 50)
                    {
                        continue;
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
                    await _addressRepository.BulkInsertAddressesIfNotExists(addresses, cancellationToken).AnyContext();
                }

                await Task.Delay(TimeSpan.FromSeconds(15), cancellationToken).AnyContext();
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
