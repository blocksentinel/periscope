using System;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Exceptions;
using Cinder.Data.Repositories;
using Cinder.Extensions;
using Cinder.Indexing.BlockIndexer.Host.Infrastructure.StepsHandlers;
using Foundatio.Utility;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nethereum.BlockchainProcessing;
using Nethereum.Web3;

namespace Cinder.Indexing.BlockIndexer.Host.Infrastructure.Hosting
{
    public class BlockIndexerHost : BackgroundService
    {
        private readonly IBlockProgressRepository _blockProgressRepository;
        private readonly CinderBlockStorageStepHandler _blockStorageStepHandler;
        private readonly CinderContractCreationStorageStepHandler _contractCreationStorageStepHandler;
        private readonly CinderFilterLogStorageStepHandler _filterLogStorageStepHandler;
        private readonly ILogger<BlockIndexerHost> _logger;
        private readonly CinderTransactionReceiptStorageStepHandler _transactionReceiptStorageStepHandler;
        private readonly IWeb3 _web3;

        public BlockIndexerHost(ILogger<BlockIndexerHost> logger, IWeb3 web3, IBlockProgressRepository blockProgressRepository,
            CinderBlockStorageStepHandler blockStorageStepHandler,
            CinderContractCreationStorageStepHandler contractCreationStorageStepHandler,
            CinderFilterLogStorageStepHandler filterLogStorageStepHandler,
            CinderTransactionReceiptStorageStepHandler transactionReceiptStorageStepHandler)
        {
            _logger = logger;
            _web3 = web3;
            _blockProgressRepository = blockProgressRepository;
            _blockStorageStepHandler = blockStorageStepHandler;
            _contractCreationStorageStepHandler = contractCreationStorageStepHandler;
            _filterLogStorageStepHandler = filterLogStorageStepHandler;
            _transactionReceiptStorageStepHandler = transactionReceiptStorageStepHandler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            BlockchainProcessor processor = _web3.Processing.Blocks.CreateBlockProcessor(_blockProgressRepository, steps =>
            {
                steps.BlockStep.AddProcessorHandler(_blockStorageStepHandler);
                steps.ContractCreationStep.AddProcessorHandler(_contractCreationStorageStepHandler);
                steps.FilterLogStep.AddProcessorHandler(_filterLogStorageStepHandler);
                steps.TransactionReceiptStep.AddProcessorHandler(_transactionReceiptStorageStepHandler);
            }, 1);

            try
            {
                await processor.ExecuteAsync(stoppingToken).AnyContext();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "{Class} -> {Method} -> Unexpected error with BlockchainProcessor", nameof(BlockIndexerHost),
                    nameof(Run));
                throw new LoggedException(e);
            }
        }
    }
}
