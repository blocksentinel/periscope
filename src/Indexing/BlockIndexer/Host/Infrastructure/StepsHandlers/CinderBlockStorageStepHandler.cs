using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Foundatio.Messaging;
using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Periscope.Core.Extensions;
using Periscope.Data.Repositories;
using Periscope.Events;

namespace Periscope.Indexing.BlockIndexer.Host.Infrastructure.StepsHandlers
{
    public class CinderBlockStorageStepHandler : BlockStorageStepHandler
    {
        private readonly IMessageBus _bus;
        private readonly ILogger<CinderBlockStorageStepHandler> _logger;

        public CinderBlockStorageStepHandler(ILogger<CinderBlockStorageStepHandler> logger, IBlockRepository blockRepository,
            IMessageBus bus) : base(blockRepository)
        {
            _logger = logger;
            _bus = bus;
        }

        protected override async Task ExecuteInternalAsync(BlockWithTransactions value)
        {
            _logger.LogDebug("Processing block {Block}", value.Number.ToUlong());
            await base.ExecuteInternalAsync(value).AnyContext();

            await _bus.PublishAsync(BlockFoundEvent.Create(value)).AnyContext();

            List<string> addresses = new() {value.Miner};
            addresses.AddRange(value.Transactions.Select(transaction => transaction.From));
            addresses.AddRange(value.Transactions.Select(transaction => transaction.To));

            await _bus.PublishAsync(AddressesTransactedEvent.Create(addresses.Distinct().ToArray())).AnyContext();
        }
    }
}
