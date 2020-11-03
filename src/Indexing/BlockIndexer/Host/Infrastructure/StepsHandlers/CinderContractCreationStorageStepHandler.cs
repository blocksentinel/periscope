using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers;
using Nethereum.RPC.Eth.DTOs;
using Periscope.Core.Extensions;
using Periscope.Data.Repositories;

namespace Periscope.Indexing.BlockIndexer.Host.Infrastructure.StepsHandlers
{
    public class CinderContractCreationStorageStepHandler : ContractCreationStorageStepHandler
    {
        private readonly IContractRepository _contractRepository;
        private readonly ILogger<CinderContractCreationStorageStepHandler> _logger;

        public CinderContractCreationStorageStepHandler(ILogger<CinderContractCreationStorageStepHandler> logger,
            IContractRepository contractRepository) : base(contractRepository)
        {
            _logger = logger;
            _contractRepository = contractRepository;
        }

        protected override async Task ExecuteInternalAsync(ContractCreationVO value)
        {
            _logger.LogDebug("Processing contract {ContractAddress}", value.ContractAddress);
            await base.ExecuteInternalAsync(value).AnyContext();
        }
    }
}
