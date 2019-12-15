using Cinder.Documents;
using MongoDB.Driver;

namespace Cinder.Data.IndexBuilders
{
    public class AddressTransactionIndexBuilder : BaseIndexBuilder<CinderAddressTransaction>
    {
        public AddressTransactionIndexBuilder(IMongoDatabase db) : base(db, CollectionName.AddressTransactions) { }

        public override void EnsureIndexes()
        {
            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderAddressTransaction>(
                Builders<CinderAddressTransaction>.IndexKeys.Combine(
                    Builders<CinderAddressTransaction>.IndexKeys.Ascending(f => f.BlockNumber),
                    Builders<CinderAddressTransaction>.IndexKeys.Ascending(f => f.Hash),
                    Builders<CinderAddressTransaction>.IndexKeys.Ascending(f => f.Address)),
                new CreateIndexOptions {Unique = true, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderAddressTransaction>(
                Builders<CinderAddressTransaction>.IndexKeys.Combine(
                    Builders<CinderAddressTransaction>.IndexKeys.Ascending(f => f.Address),
                    Builders<CinderAddressTransaction>.IndexKeys.Descending(f => f.BlockNumber)),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderAddressTransaction>(
                Builders<CinderAddressTransaction>.IndexKeys.Ascending(f => f.Hash),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderAddressTransaction>(
                Builders<CinderAddressTransaction>.IndexKeys.Ascending(f => f.Address),
                new CreateIndexOptions {Unique = false, Background = true}));
        }
    }
}
