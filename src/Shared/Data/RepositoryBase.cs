using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Periscope.Core.Extensions;
using Periscope.Data.Extensions;
using Periscope.Documents;

namespace Periscope.Data
{
    public abstract class RepositoryBase<TDocument> : IRepository where TDocument : IDocument
    {
        protected readonly IMongoClient Client;
        protected readonly IMongoCollection<TDocument> Collection;
        protected readonly IMongoDatabase Database;

        protected RepositoryBase(IMongoClient client, string databaseName, CollectionName collectionName)
        {
            Client = client;
            Database = Client.GetDatabase(databaseName);
            Collection = Database.GetCollection<TDocument>(collectionName.ToCollectionName(),
                new MongoCollectionSettings {ReadEncoding = new UTF8Encoding(false, false)});
        }

        protected virtual FilterDefinition<TDocument> CreateDocumentFilter(string id)
        {
            id = id.ToLowerInvariant();

            return Builders<TDocument>.Filter.Eq(document => document.Id, id);
        }

        protected virtual FilterDefinition<TDocument> CreateDocumentFilter(TDocument entity)
        {
            return Builders<TDocument>.Filter.Eq(document => document.Id, entity.Id);
        }

        protected async Task UpsertDocumentAsync(TDocument updatedDocument, CancellationToken cancellationToken = default)
        {
            await Collection.ReplaceOneAsync(CreateDocumentFilter(updatedDocument), updatedDocument,
                    new ReplaceOptions {IsUpsert = true}, cancellationToken)
                .AnyContext();
        }

        protected async Task<int> BulkUpsertDocumentAsync(IEnumerable<TDocument> updatedDocuments,
            CancellationToken cancellationToken = default)
        {
            List<WriteModel<TDocument>> requests = updatedDocuments
                .Select(document =>
                    new ReplaceOneModel<TDocument>(Builders<TDocument>.Filter.Where(x => x.Id == document.Id), document)
                    {
                        IsUpsert = true
                    })
                .Cast<WriteModel<TDocument>>()
                .ToList();

            BulkWriteResult<TDocument> result = await Collection.BulkWriteAsync(requests, cancellationToken: cancellationToken)
                .AnyContext();

            return result.ProcessedRequests.Count;
        }
    }
}
