using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Documents;
using Cinder.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;

namespace Cinder.Data.Repositories
{
    public class AddressTransactionRepository : RepositoryBase<CinderAddressTransaction>, IAddressTransactionRepository
    {
        public AddressTransactionRepository(IMongoClient client, string databaseName) : base(client, databaseName,
            CollectionName.AddressTransactions) { }

        public Task UpsertAsync(TransactionReceiptVO transactionReceiptVO, string address, string error = null,
            string newContractAddress = null)
        {
            return UpsertDocumentAsync(transactionReceiptVO.MapToStorageEntityForUpsert<CinderAddressTransaction>(address));
        }

        public async Task<IAddressTransactionView> FindAsync(string address, HexBigInteger blockNumber, string transactionHash)
        {
            address = address.ToLowerInvariant();
            transactionHash = transactionHash.ToLowerInvariant();
            FilterDefinition<CinderAddressTransaction> filter = CreateDocumentFilter(new CinderAddressTransaction
            {
                Address = address, Hash = transactionHash, BlockNumber = blockNumber.Value.ToString()
            });
            CinderAddressTransaction response = await Collection.Find(filter).SingleOrDefaultAsync().AnyContext();

            return response;
        }

        public async Task<IEnumerable<string>> GetUniqueAddresses(CancellationToken cancellationToken)
        {
            using IAsyncCursor<string> t = await Collection
                .DistinctAsync(field => field.Address, new BsonDocument(), cancellationToken: cancellationToken)
                .AnyContext();

            List<string> records = new List<string>();
            while (await t.MoveNextAsync(cancellationToken).AnyContext()) records.AddRange(t.Current);

            return records;
        }

        public async Task<IEnumerable<string>> GetTransactionHashesByAddressHash(string addressHash, int? size = null,
            CancellationToken cancellationToken = default)
        {
            size ??= 10;

            List<string> transactions = await AddressHashBaseQuery(addressHash)
                .Limit(size.Value)
                .SortByDescending(transaction => transaction.BlockNumber)
                .Project(document => document.Hash)
                .ToListAsync(cancellationToken)
                .AnyContext();

            return transactions;
        }

        public async Task<IPage<string>> GetPagedTransactionHashesByAddressHash(string addressHash, int? page = null,
            int? size = null, SortOrder sort = SortOrder.Ascending, CancellationToken cancellationToken = default)
        {
            page ??= 1;
            size ??= 10;

            IFindFluent<CinderAddressTransaction, CinderAddressTransaction> query = AddressHashBaseQuery(addressHash);
            // NOTE: Hard cap to 10k records to avoid performance issues, needs further investigation
            long total = await query.Limit(10000).CountDocumentsAsync(cancellationToken).AnyContext();
            query = query.Skip((page.Value - 1) * size.Value).Limit(size.Value);

            switch (sort)
            {
                case SortOrder.Ascending:
                    query = query.SortBy(transaction => transaction.BlockNumber);
                    break;
                case SortOrder.Descending:
                    query = query.SortByDescending(transaction => transaction.BlockNumber);
                    break;
            }

            List<string> transactions =
                await query.Project(document => document.Hash).ToListAsync(cancellationToken).AnyContext();

            return new PagedEnumerable<string>(transactions, (int) total, page.Value, size.Value);
        }

        public async Task<ulong> GetTransactionCountByAddressHash(string addressHash,
            CancellationToken cancellationToken = default)
        {
            IFindFluent<CinderAddressTransaction, CinderAddressTransaction> query = AddressHashBaseQuery(addressHash);
            // NOTE: Hard cap to 10k records to avoid performance issues, needs further investigation
            long total = await query.Limit(10000).CountDocumentsAsync(cancellationToken).AnyContext();

            return (ulong) total;
        }

        private IFindFluent<CinderAddressTransaction, CinderAddressTransaction> AddressHashBaseQuery(string addressHash)
        {
            addressHash = addressHash.ToLowerInvariant();

            return Collection.Find(Builders<CinderAddressTransaction>.Filter.Eq(transaction => transaction.Address, addressHash));
        }
    }
}
