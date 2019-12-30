using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Extensions;
using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing.BlockStorage.BlockStorageStepsHandlers;
using Nethereum.RPC.Eth.DTOs;

namespace Cinder.Indexers.BlockIndexer.Host.Infrastructure.StepsHandlers
{
    public class CinderTransactionReceiptStorageStepHandler : TransactionReceiptStorageStepHandler
    {
        private readonly ILogger<CinderTransactionReceiptStorageStepHandler> _logger;

        public CinderTransactionReceiptStorageStepHandler(ILogger<CinderTransactionReceiptStorageStepHandler> logger,
            ITransactionRepository transactionRepository, IAddressTransactionRepository addressTransactionRepository = null) :
            base(transactionRepository, addressTransactionRepository)
        {
            _logger = logger;
        }

        protected override async Task ExecuteInternalAsync(TransactionReceiptVO value)
        {
            _logger.LogInformation("Processing transaction receipt");
            await base.ExecuteInternalAsync(value).AnyContext();
        }
    }
}
