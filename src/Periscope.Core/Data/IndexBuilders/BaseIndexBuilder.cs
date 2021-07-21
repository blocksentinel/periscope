using MongoDB.Driver;
using Periscope.Core.Documents;
using Periscope.Core.Extensions;

namespace Periscope.Core.Data.IndexBuilders
{
    public abstract class BaseIndexBuilder<TDocument> : IIndexBuilder where TDocument : IDocument
    {
        protected readonly IMongoCollection<TDocument> Collection;

        protected BaseIndexBuilder(IMongoDatabase db, CollectionName collectionName)
        {
            Collection = db.GetCollection<TDocument>(collectionName.ToCollectionName());
        }

        public abstract void EnsureIndexes();
    }
}
