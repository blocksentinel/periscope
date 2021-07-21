using MongoDB.Driver;
using Periscope.Core.Documents;

namespace Periscope.Core.Data.IndexBuilders
{
    public class ContractIndexBuilder : BaseIndexBuilder<CinderContract>
    {
        public ContractIndexBuilder(IMongoDatabase db) : base(db, CollectionName.Contracts) { }

        public override void EnsureIndexes()
        {
            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderContract>(
                Builders<CinderContract>.IndexKeys.Ascending(f => f.Name),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderContract>(
                Builders<CinderContract>.IndexKeys.Ascending(f => f.Name),
                new CreateIndexOptions {Unique = true, Background = true}));
        }
    }
}
