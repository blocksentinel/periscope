using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Exceptions;
using Cinder.Core.Paging;
using Cinder.Documents;
using Cinder.Extensions;
using MongoDB.Driver;

namespace Cinder.Data.Repositories
{
    public class AddressRepository : RepositoryBase<CinderAddress>, IAddressRepository
    {
        public AddressRepository(IMongoClient client, string databaseName) : base(client, databaseName,
            CollectionName.Addresses) { }

        public async Task BulkInsertAddressesIfNotExists(IEnumerable<CinderAddress> addresses,
            CancellationToken cancellationToken = default)
        {
            List<WriteModel<CinderAddress>> requests = addresses.Select(document => new InsertOneModel<CinderAddress>(document))
                .Cast<WriteModel<CinderAddress>>()
                .ToList();

            // TODO 20191218 RJ This probably can be handled better
            try
            {
                await Collection.BulkWriteAsync(requests, new BulkWriteOptions {IsOrdered = false}, cancellationToken)
                    .AnyContext();
            }
            catch (MongoBulkWriteException e)
            {
                if (e.WriteErrors.Any(error => error.Category != ServerErrorCategory.DuplicateKey))
                {
                    throw;
                }

                throw new LoggedException(e);
            }
        }

        public async Task UpsertAddress(CinderAddress address, CancellationToken cancellationToken = default)
        {
            await UpsertDocumentAsync(address, cancellationToken).AnyContext();
        }

        public async Task BulkUpsertAddresses(IEnumerable<CinderAddress> addresses, CancellationToken cancellationToken = default)
        {
            await BulkUpsertDocumentAsync(addresses, cancellationToken).AnyContext();
        }

        public async Task<CinderAddress> GetAddressByHash(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();

            return await Collection.Find(Builders<CinderAddress>.Filter.Eq(document => document.Hash, hash))
                .SingleOrDefaultAsync(cancellationToken)
                .AnyContext();
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
            // TODO 20191226 RJ This needs to be calculated/retreived in a different way, it is currently inefficient
            var result = await Collection.Aggregate()
                .Group(address => address.Temp, group => new {Balance = group.Sum(y => y.Balance)})
                .ToListAsync(cancellationToken);

            return result.FirstOrDefault()?.Balance ?? 0;
        }
    }
}
