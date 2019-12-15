using Cinder.Documents;
using MongoDB.Driver;

namespace Cinder.Data.IndexBuilders
{
    public class TransactionIndexBuilder : BaseIndexBuilder<CinderTransaction>
    {
        public TransactionIndexBuilder(IMongoDatabase db) : base(db, CollectionName.Transactions) { }

        public override void EnsureIndexes()
        {
            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransaction>(
                Builders<CinderTransaction>.IndexKeys.Ascending(f => f.BlockHash),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransaction>(
                Builders<CinderTransaction>.IndexKeys.Ascending(f => f.Hash),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransaction>(
                Builders<CinderTransaction>.IndexKeys.Ascending(f => f.AddressFrom),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransaction>(
                Builders<CinderTransaction>.IndexKeys.Ascending(f => f.AddressTo),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransaction>(
                Builders<CinderTransaction>.IndexKeys.Ascending(f => f.NewContractAddress),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransaction>(
                Builders<CinderTransaction>.IndexKeys.Ascending(f => f.TimeStamp),
                new CreateIndexOptions {Unique = false, Background = true}));
        }
    }
}
