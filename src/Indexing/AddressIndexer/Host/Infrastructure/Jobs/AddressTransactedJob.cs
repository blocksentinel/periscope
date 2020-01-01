using System;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Documents;
using Cinder.Extensions;
using Foundatio.Jobs;
using Foundatio.Queues;
using Microsoft.Extensions.Logging;

namespace Cinder.Indexing.AddressIndexer.Host.Infrastructure.Jobs
{
    public class AddressTransactedJob : QueueJobBase<AddressTransactedWorkItem>
    {
        private readonly IAddressRepository _addressRepository;

        public AddressTransactedJob(IQueue<AddressTransactedWorkItem> queue, ILoggerFactory loggerFactory,
            IAddressRepository addressRepository) : base(queue, loggerFactory)
        {
            _addressRepository = addressRepository;
        }

        protected override async Task<JobResult> ProcessQueueEntryAsync(QueueEntryContext<AddressTransactedWorkItem> context)
        {
            string address = context.QueueEntry.Value.Address;

            try
            {
                if (await _addressRepository.AddressExists(address).AnyContext())
                {
                    return JobResult.Success;
                }

                await _addressRepository.UpsertAddress(new CinderAddress {Hash = address, ForceRefresh = true}).AnyContext();
            }
            catch (Exception e)
            {
                return JobResult.FromException(e);
            }

            return JobResult.Success;
        }
    }
}
