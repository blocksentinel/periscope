using Cinder.Documents;
using MongoDB.Driver;

namespace Cinder.Data.IndexBuilders
{
    public class AddressIndexBuilder : BaseIndexBuilder<CinderAddress>
    {
        public AddressIndexBuilder(IMongoDatabase db) : base(db, CollectionName.Addresses) { }

        public override void EnsureIndexes()
        {
            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderAddress>(
                Builders<CinderAddress>.IndexKeys.Ascending(f => f.Hash),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderAddress>(
                Builders<CinderAddress>.IndexKeys.Ascending(f => f.Balance),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderAddress>(
                Builders<CinderAddress>.IndexKeys.Ascending(f => f.Timestamp),
                new CreateIndexOptions {Unique = false, Background = true}));
        }
    }
}
