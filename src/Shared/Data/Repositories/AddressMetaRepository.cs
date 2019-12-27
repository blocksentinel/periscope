using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Documents;
using Cinder.Extensions;
using MongoDB.Driver;

namespace Cinder.Data.Repositories
{
    public class AddressMetaRepository : RepositoryBase<CinderAddressMeta>, IAddressMetaRepository
    {
        public AddressMetaRepository(IMongoClient client, string databaseName) :
            base(client, databaseName, CollectionName.AddressMeta) { }

        public async Task<CinderAddressMeta> GetByAddressOrDefault(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();

            return await Collection.Find(Builders<CinderAddressMeta>.Filter.Eq(document => document.Hash, hash))
                .SingleOrDefaultAsync(cancellationToken)
                .AnyContext();
        }

        public async Task<IEnumerable<CinderAddressMeta>> GetByAddresses(IEnumerable<string> hashes,
            CancellationToken cancellationToken = default)
        {
            return await Collection.Find(Builders<CinderAddressMeta>.Filter.In(document => document.Hash, hashes))
                .ToListAsync(cancellationToken)
                .AnyContext();
        }
    }
}
