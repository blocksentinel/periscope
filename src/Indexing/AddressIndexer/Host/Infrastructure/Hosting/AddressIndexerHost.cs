using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Events;
using Cinder.Extensions;
using Cinder.Indexing.AddressIndexer.Host.Infrastructure.Jobs;
using Foundatio.Jobs;
using Foundatio.Messaging;
using Foundatio.Queues;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.Parity;
using StackExchange.Redis;

namespace Cinder.Indexing.AddressIndexer.Host.Infrastructure.Hosting
{
    public class AddressIndexerHost : BackgroundService
    {
        private readonly AddressRefresherJob _addressRefresherJob;
        private readonly AddressTransactedJob _addressTransactedJob;
        private readonly IMessageBus _bus;
        private readonly IQueue<AddressTransactedWorkItem> _queue;

        public AddressIndexerHost(ILoggerFactory loggerFactory, IMessageBus bus, IConnectionMultiplexer muxer,
            IQueue<AddressTransactedWorkItem> queue, IAddressRepository addressRepository, IWeb3Parity web3)
        {
            _bus = bus;
            _queue = queue;
            _addressTransactedJob = new AddressTransactedJob(_queue, loggerFactory, addressRepository);
            _addressRefresherJob = new AddressRefresherJob(loggerFactory, muxer, addressRepository, web3);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            List<Task> tasks = new List<Task>
            {
                _bus.SubscribeAsync<AddressesTransactedEvent>(async @event =>
                {
                    foreach (string address in @event.Addresses)
                    {
                        await _queue.EnqueueAsync(new AddressTransactedWorkItem {Address = address}).AnyContext();
                    }
                }, stoppingToken),
                _addressTransactedJob.RunContinuousAsync(cancellationToken: stoppingToken),
                _addressRefresherJob.RunContinuousAsync(TimeSpan.FromSeconds(15), cancellationToken: stoppingToken)
            };

            await Task.WhenAll(tasks).AnyContext();
        }

        public override void Dispose()
        {
            _addressRefresherJob?.Dispose();
            base.Dispose();
        }
    }
}
