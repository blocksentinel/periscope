using System.Threading.Tasks;
using MongoDB.Driver;
using Periscope.Data;

namespace Periscope.Indexing.HostBase.Repositories
{
    public interface IIndexerRepositoryFactory : IRepositoryFactory
    {
        IMongoDatabase CreateDbIfNotExists();
        Task DeleteDatabase();
        Task CreateCollectionsIfNotExist(IMongoDatabase db, string locale);
        Task DeleteAllCollections(IMongoDatabase db);
    }
}
