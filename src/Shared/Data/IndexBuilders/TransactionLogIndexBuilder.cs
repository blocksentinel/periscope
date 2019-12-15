using Cinder.Documents;
using MongoDB.Driver;

namespace Cinder.Data.IndexBuilders
{
    public class TransactionLogIndexBuilder : BaseIndexBuilder<CinderTransactionLog>
    {
        public TransactionLogIndexBuilder(IMongoDatabase db) : base(db, CollectionName.TransactionLogs) { }

        public override void EnsureIndexes()
        {
            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransactionLog>(
                Builders<CinderTransactionLog>.IndexKeys.Combine(
                    Builders<CinderTransactionLog>.IndexKeys.Ascending(f => f.TransactionHash),
                    Builders<CinderTransactionLog>.IndexKeys.Ascending(f => f.LogIndex)),
                new CreateIndexOptions {Unique = true, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransactionLog>(
                Builders<CinderTransactionLog>.IndexKeys.Ascending(f => f.Address),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransactionLog>(
                Builders<CinderTransactionLog>.IndexKeys.Ascending(f => f.EventHash),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransactionLog>(
                Builders<CinderTransactionLog>.IndexKeys.Ascending(f => f.IndexVal1),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransactionLog>(
                Builders<CinderTransactionLog>.IndexKeys.Ascending(f => f.IndexVal2),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderTransactionLog>(
                Builders<CinderTransactionLog>.IndexKeys.Ascending(f => f.IndexVal3),
                new CreateIndexOptions {Unique = false, Background = true}));
        }
    }
}
