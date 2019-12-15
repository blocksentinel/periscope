using System.Threading.Tasks;
using Cinder.Data;
using MongoDB.Driver;

namespace Cinder.Indexer.Host.Infrastructure.Repositories
{
    public interface IIndexerRepositoryFactory : IRepositoryFactory
    {
        IMongoDatabase CreateDbIfNotExists();
        Task DeleteDatabase();
        Task CreateCollectionsIfNotExist(IMongoDatabase db, string locale);
        Task DeleteAllCollections(IMongoDatabase db);
    }
}
