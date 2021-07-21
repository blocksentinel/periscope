using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Periscope.Core.Documents;
using Periscope.Core.Extensions;

namespace Periscope.Core.Data.Repositories
{
    public class AddressMetaRepository : RepositoryBase<CinderAddressMeta>, IAddressMetaRepository
    {
        public AddressMetaRepository(IMongoClient client, string databaseName) : base(client, databaseName,
            CollectionName.AddressMeta) { }

        public Task<CinderAddressMeta> GetByAddressOrDefault(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();

            return Collection.Find(Builders<CinderAddressMeta>.Filter.Eq(document => document.Hash, hash))
                .SingleOrDefaultAsync(cancellationToken);
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
