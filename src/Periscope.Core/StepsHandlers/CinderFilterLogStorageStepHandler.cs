using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers;
using Nethereum.RPC.Eth.DTOs;
using Periscope.Core.Data.Repositories;
using Periscope.Core.Extensions;

namespace Periscope.Core.StepsHandlers
{
    public class CinderFilterLogStorageStepHandler : FilterLogStorageStepHandler
    {
        private readonly ILogger<CinderFilterLogStorageStepHandler> _logger;
        private readonly ITransactionLogRepository _transactionLogRepository;

        public CinderFilterLogStorageStepHandler(ILogger<CinderFilterLogStorageStepHandler> logger,
            ITransactionLogRepository transactionLogRepository) : base(transactionLogRepository)
        {
            _logger = logger;
            _transactionLogRepository = transactionLogRepository;
        }

        protected override async Task ExecuteInternalAsync(FilterLogVO value)
        {
            _logger.LogDebug("Processing filter log");
            await base.ExecuteInternalAsync(value).AnyContext();
        }
    }
}
