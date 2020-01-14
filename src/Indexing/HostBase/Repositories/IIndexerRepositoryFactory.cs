using System.Threading.Tasks;
using Cinder.Data;
using MongoDB.Driver;

namespace Cinder.Indexing.HostBase.Repositories
{
    public interface IIndexerRepositoryFactory : IRepositoryFactory
    {
        IMongoDatabase CreateDbIfNotExists();
        Task DeleteDatabase();
        Task CreateCollectionsIfNotExist(IMongoDatabase db, string locale);
        Task DeleteAllCollections(IMongoDatabase db);
    }
}
