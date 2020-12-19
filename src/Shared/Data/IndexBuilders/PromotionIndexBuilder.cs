using MongoDB.Driver;
using Periscope.Documents;

namespace Periscope.Data.IndexBuilders
{
    public class PromotionIndexBuilder : BaseIndexBuilder<Promotion>
    {
        public PromotionIndexBuilder(IMongoDatabase db) : base(db, CollectionName.Promotion) { }

        public override void EnsureIndexes()
        {
            Collection.Indexes.CreateOneAsync(new CreateIndexModel<Promotion>(
                Builders<Promotion>.IndexKeys.Ascending(f => f.Active),
                new CreateIndexOptions {Unique = false, Background = true}));
        }
    }
}
