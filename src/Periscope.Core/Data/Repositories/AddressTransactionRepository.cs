using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Periscope.Core.Documents;
using Periscope.Core.Extensions;
using Periscope.Core.Paging;

namespace Periscope.Core.Data.Repositories
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

            List<string> records = new();
            while (await t.MoveNextAsync(cancellationToken).AnyContext()) records.AddRange(t.Current);

            return records;
        }

        public async Task<IPage<string>> GetPagedTransactionHashesByAddressHash(string addressHash, int? page = null,
            int? size = null, SortOrder sort = SortOrder.Ascending, int queryCountLimiter = 10000,
            CancellationToken cancellationToken = default)
        {
            page ??= 1;
            size ??= 10;

            IFindFluent<CinderAddressTransaction, CinderAddressTransaction> query = AddressHashBaseQuery(addressHash);
            long total = await query.Limit(queryCountLimiter).CountDocumentsAsync(cancellationToken).AnyContext();

            query = sort switch
            {
                SortOrder.Ascending => query.SortBy(transaction => transaction.BlockNumber),
                SortOrder.Descending => query.SortByDescending(transaction => transaction.BlockNumber),
                _ => throw new NotImplementedException()
            };

            query = query.Skip((page.Value - 1) * size.Value).Limit(size.Value);

            List<string> transactions =
                await query.Project(document => document.Hash).ToListAsync(cancellationToken).AnyContext();

            return new PagedEnumerable<string>(transactions, (int) total, page.Value, size.Value);
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

        public async Task<ulong> GetTransactionCountByAddressHash(string addressHash, int queryCountLimiter = 10000,
            CancellationToken cancellationToken = default)
        {
            IFindFluent<CinderAddressTransaction, CinderAddressTransaction> query = AddressHashBaseQuery(addressHash);
            long total = await query.Limit(queryCountLimiter).CountDocumentsAsync(cancellationToken).AnyContext();

            return (ulong) total;
        }

        private IFindFluent<CinderAddressTransaction, CinderAddressTransaction> AddressHashBaseQuery(string addressHash)
        {
            addressHash = addressHash.ToLowerInvariant();

            return Collection.Find(Builders<CinderAddressTransaction>.Filter.Eq(transaction => transaction.Address, addressHash));
        }
    }
}
