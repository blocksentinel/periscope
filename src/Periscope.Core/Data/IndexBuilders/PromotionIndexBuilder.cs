using MongoDB.Driver;
using Periscope.Core.Documents;

namespace Periscope.Core.Data.IndexBuilders
{
    public class PromotionIndexBuilder : BaseIndexBuilder<Promotion>
    {
        public PromotionIndexBuilder(IMongoDatabase db) : base(db, CollectionName.Promotions) { }

        public override void EnsureIndexes()
        {
            Collection.Indexes.CreateOneAsync(new CreateIndexModel<Promotion>(
                Builders<Promotion>.IndexKeys.Ascending(f => f.Active),
                new CreateIndexOptions {Unique = false, Background = true}));
        }
    }
}
