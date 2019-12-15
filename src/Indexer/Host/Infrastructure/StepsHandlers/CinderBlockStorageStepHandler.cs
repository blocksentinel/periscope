using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Events;
using Cinder.Extensions;
using Foundatio.Messaging;
using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Cinder.Indexer.Host.Infrastructure.StepsHandlers
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
            _logger.LogInformation("Processing block {Block}", value.Number.ToUlong());
            await base.ExecuteInternalAsync(value).AnyContext();
            await _bus.PublishAsync(BlockFoundEvent.Create(value.Number.ToUlong())).AnyContext();
        }
    }
}
