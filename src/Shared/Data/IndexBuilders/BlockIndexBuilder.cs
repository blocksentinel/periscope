using Cinder.Documents;
using MongoDB.Driver;

namespace Cinder.Data.IndexBuilders
{
    public class BlockIndexBuilder : BaseIndexBuilder<CinderBlock>
    {
        public BlockIndexBuilder(IMongoDatabase db) : base(db, CollectionName.Blocks) { }

        public override void EnsureIndexes()
        {
            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderBlock>(
                Builders<CinderBlock>.IndexKeys.Combine(Builders<CinderBlock>.IndexKeys.Ascending(f => f.BlockNumber),
                    Builders<CinderBlock>.IndexKeys.Ascending(f => f.Hash)),
                new CreateIndexOptions {Unique = true, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderBlock>(
                Builders<CinderBlock>.IndexKeys.Ascending(f => f.Hash),
                new CreateIndexOptions {Unique = false, Background = true}));

            Collection.Indexes.CreateOneAsync(new CreateIndexModel<CinderBlock>(
                Builders<CinderBlock>.IndexKeys.Ascending(f => f.Miner),
                new CreateIndexOptions {Unique = false, Background = true}));
        }
    }
}
