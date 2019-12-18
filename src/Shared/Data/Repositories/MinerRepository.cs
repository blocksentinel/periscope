using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Documents;
using Cinder.Extensions;
using MongoDB.Driver;

namespace Cinder.Data.Repositories
{
    public class MinerRepository : RepositoryBase<CinderMiner>, IMinerRepository
    {
        public MinerRepository(IMongoClient client, string databaseName) : base(client, databaseName, CollectionName.Miner) { }

        public async Task<CinderMiner> GetByAddressOrDefault(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();

            return await Collection.Find(Builders<CinderMiner>.Filter.Eq(document => document.Hash, hash))
                .SingleOrDefaultAsync(cancellationToken)
                .AnyContext();
        }

        public async Task<IEnumerable<CinderMiner>> GetByAddresses(IEnumerable<string> hashes,
            CancellationToken cancellationToken = default)
        {
            return await Collection.Find(Builders<CinderMiner>.Filter.In(document => document.Hash, hashes))
                .ToListAsync(cancellationToken)
                .AnyContext();
        }
    }
}
