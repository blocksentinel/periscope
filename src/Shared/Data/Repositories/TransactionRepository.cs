using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Documents;
using Cinder.Extensions;
using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Cinder.Data.Repositories
{
    public class TransactionRepository : RepositoryBase<CinderTransaction>, ITransactionRepository
    {
        public TransactionRepository(IMongoClient client, string databaseName) : base(client, databaseName,
            CollectionName.Transactions) { }

        public async Task<ITransactionView> FindByBlockNumberAndHashAsync(HexBigInteger blockNumber, string hash)
        {
            hash = hash.ToLowerInvariant();
            FilterDefinition<CinderTransaction> filter = CreateDocumentFilter(new CinderTransaction
            {
                BlockNumber = blockNumber.Value.ToString(), Hash = hash
            });
            CinderTransaction response = await Collection.Find(filter).SingleOrDefaultAsync().AnyContext();

            return response;
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string code, bool failedCreatingContract)
        {
            await UpsertDocumentAsync(
                    transactionReceiptVO.MapToStorageEntityForUpsert<CinderTransaction>(code, failedCreatingContract))
                .AnyContext();
        }

        public async Task UpsertAsync(TransactionReceiptVO transactionReceiptVO)
        {
            await UpsertDocumentAsync(transactionReceiptVO.MapToStorageEntityForUpsert<CinderTransaction>()).AnyContext();
        }

        public async Task<IPage<CinderTransaction>> GetTransactions(int? page = null, int? size = null,
            SortOrder sort = SortOrder.Ascending, CancellationToken cancellationToken = default)
        {
            long total = await Collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken).AnyContext();
            IFindFluent<CinderTransaction, CinderTransaction> query = Collection.Find(FilterDefinition<CinderTransaction>.Empty)
                .Skip(((page ?? 1) - 1) * (size ?? 10))
                .Limit(size ?? 10);

            switch (sort)
            {
                case SortOrder.Ascending:
                    query = query.SortBy(transaction => transaction.TimeStamp);
                    break;
                case SortOrder.Descending:
                    query = query.SortByDescending(transaction => transaction.TimeStamp);
                    break;
            }

            List<CinderTransaction> transactions = await query.ToListAsync(cancellationToken).AnyContext();

            return new PagedEnumerable<CinderTransaction>(transactions, (int) total, page ?? 1, size ?? 10);
        }

        public async Task<CinderTransaction> GetTransactionByHash(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();

            return await Collection.Find(Builders<CinderTransaction>.Filter.Eq(document => document.Hash, hash))
                .SingleOrDefaultAsync(cancellationToken)
                .AnyContext();
        }

        public async Task<IEnumerable<CinderTransaction>> GetTransactionsByBlockHash(string blockHash,
            CancellationToken cancellationToken = default)
        {
            blockHash = blockHash.ToLowerInvariant();

            return await Collection.Find(Builders<CinderTransaction>.Filter.Eq(document => document.BlockHash, blockHash))
                .ToListAsync(cancellationToken)
                .AnyContext();
        }

        public async Task<string> GetTransactionHashIfExists(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();
            var result = await Collection.Find(Builders<CinderTransaction>.Filter.Eq(document => document.Hash, hash))
                .Project(transaction => new {transaction.Hash})
                .SingleOrDefaultAsync(cancellationToken)
                .AnyContext();

            return result.Hash;
        }

        public async Task<IEnumerable<CinderTransaction>> GetTransactionsByHashes(IEnumerable<string> transactionHashes,
            CancellationToken cancellationToken = default)
        {
            return await Collection.Find(Builders<CinderTransaction>.Filter.In(document => document.Hash, transactionHashes))
                .SortByDescending(document => document.BlockNumber)
                .ToListAsync(cancellationToken)
                .AnyContext();
        }
    }
}
