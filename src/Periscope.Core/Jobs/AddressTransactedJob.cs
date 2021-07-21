using System;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Jobs;
using Foundatio.Queues;
using Microsoft.Extensions.Logging;
using Periscope.Core.Data.Repositories;
using Periscope.Core.Documents;
using Periscope.Core.Extensions;

namespace Periscope.Core.Jobs
{
    public class AddressTransactedJob : QueueJobBase<AddressTransactedWorkItem>
    {
        private readonly InMemoryCacheClient _addressCache;
        private readonly IAddressRepository _addressRepository;

        public AddressTransactedJob(IQueue<AddressTransactedWorkItem> queue, ILoggerFactory loggerFactory,
            IAddressRepository addressRepository) : base(queue, loggerFactory)
        {
            _addressCache = new InMemoryCacheClient(builder => builder.MaxItems(500));
            _addressRepository = addressRepository;
        }

        protected override async Task<JobResult> ProcessQueueEntryAsync(QueueEntryContext<AddressTransactedWorkItem> context)
        {
            string address = context.QueueEntry.Value.Address;

            if (string.IsNullOrEmpty(address))
            {
                return JobResult.SuccessWithMessage("Address was empty (maybe a contract deployment?), skipping");
            }

            if (await _addressCache.ExistsAsync(address).AnyContext())
            {
                return JobResult.SuccessWithMessage($"Address {address} exists in cache, skipping");
            }

            try
            {
                if (!await _addressRepository.AddressExists(address).AnyContext())
                {
                    await _addressRepository.UpsertAddress(new CinderAddress {Hash = address, ForceRefresh = true}).AnyContext();
                }

                await _addressCache.AddAsync(address, string.Empty).AnyContext();
            }
            catch (Exception e)
            {
                return JobResult.FromException(e);
            }

            return JobResult.Success;
        }
    }
}
