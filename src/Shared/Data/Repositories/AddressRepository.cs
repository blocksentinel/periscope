using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Documents;
using Cinder.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Cinder.Data.Repositories
{
    public class AddressRepository : RepositoryBase<CinderAddress>, IAddressRepository
    {
        public AddressRepository(IMongoClient client, string databaseName) : base(client, databaseName,
            CollectionName.Addresses) { }

        public Task UpsertAddress(CinderAddress address, CancellationToken cancellationToken = default)
        {
            return UpsertDocumentAsync(address, cancellationToken);
        }

        public Task BulkUpsertAddresses(IEnumerable<CinderAddress> addresses, CancellationToken cancellationToken = default)
        {
            return BulkUpsertDocumentAsync(addresses, cancellationToken);
        }

        public Task<CinderAddress> GetAddressByHash(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();

            return Collection.Find(Builders<CinderAddress>.Filter.Eq(document => document.Hash, hash))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<string> GetAddressHashIfExists(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();
            var result = await Collection.Find(Builders<CinderAddress>.Filter.Eq(document => document.Hash, hash))
                .Project(address => new {address.Hash})
                .SingleOrDefaultAsync(cancellationToken)
                .AnyContext();

            return result?.Hash;
        }

        public async Task<IEnumerable<CinderAddress>> GetStaleAddresses(int age = 5, int limit = 1000,
            CancellationToken cancellationToken = default)
        {
            List<CinderAddress> staleAddresses = await Collection
                .Find(Builders<CinderAddress>.Filter.Where(document => document.Timestamp == null))
                .Limit(limit)
                .ToListAsync(cancellationToken)
                .AnyContext();

            if (staleAddresses.Count < limit)
            {
                List<CinderAddress> forceRefresh = await Collection
                    .Find(Builders<CinderAddress>.Filter.Where(document => document.ForceRefresh))
                    .Limit(limit - staleAddresses.Count)
                    .SortBy(document => document.Timestamp)
                    .ToListAsync(cancellationToken)
                    .AnyContext();
                staleAddresses.AddRange(forceRefresh);
            }

            if (staleAddresses.Count >= limit)
            {
                return staleAddresses;
            }

            ulong ageLimit = (ulong) DateTimeOffset.UtcNow.AddMinutes(-Math.Abs(age)).ToUnixTimeSeconds();
            List<CinderAddress> needsRefresh = await Collection
                .Find(Builders<CinderAddress>.Filter.Where(
                    document => document.Timestamp != null && document.Timestamp < ageLimit))
                .Limit(limit - staleAddresses.Count)
                .SortBy(document => document.Timestamp)
                .ToListAsync(cancellationToken)
                .AnyContext();
            staleAddresses.AddRange(needsRefresh);

            return staleAddresses;
        }

        public async Task<IPage<CinderAddress>> GetRichest(int? page, int? size, CancellationToken cancellationToken = default)
        {
            IFindFluent<CinderAddress, CinderAddress> query = Collection.Find(FilterDefinition<CinderAddress>.Empty)
                .Skip(((page ?? 1) - 1) * (size ?? 10))
                .Limit(size ?? 10)
                .SortByDescending(document => document.Balance);
            List<CinderAddress> addresses = await query.ToListAsync(cancellationToken).AnyContext();

            return new PagedEnumerable<CinderAddress>(addresses, 1000, page ?? 1, size ?? 10);
        }

        public async Task<decimal> GetSupply(CancellationToken cancellationToken = default)
        {
            List<BsonDocument> result = await Collection.Aggregate()
                .AppendStage(new BsonDocumentPipelineStageDefinition<CinderAddress, BsonDocument>(new BsonDocument("$group",
                    new BsonDocument {{"_id", BsonNull.Value}, {"Supply", new BsonDocument("$sum", "$Balance")}})))
                .ToListAsync(cancellationToken)
                .AnyContext();

            return result.FirstOrDefault()?["Supply"].AsDecimal ?? 0;
        }

        public async Task<bool> AddressExists(string hash, CancellationToken cancellationToken = default)
        {
            long count = await Collection.Find(address => address.Hash == hash)
                .Limit(1)
                .CountDocumentsAsync(cancellationToken)
                .AnyContext();

            return count == 1;
        }
    }
}
