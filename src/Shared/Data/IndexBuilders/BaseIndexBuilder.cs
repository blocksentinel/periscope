using System.Text;
using MongoDB.Driver;
using Periscope.Data.Extensions;
using Periscope.Documents;

namespace Periscope.Data.IndexBuilders
{
    public abstract class BaseIndexBuilder<TDocument> : IIndexBuilder where TDocument : IDocument
    {
        protected readonly IMongoCollection<TDocument> Collection;

        protected BaseIndexBuilder(IMongoDatabase db, CollectionName collectionName)
        {
            Collection = db.GetCollection<TDocument>(collectionName.ToCollectionName(),
                new MongoCollectionSettings {ReadEncoding = new UTF8Encoding(false, false)});
        }

        public abstract void EnsureIndexes();
    }
}
