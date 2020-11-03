using System.Numerics;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Periscope.Core.Extensions;
using Periscope.Documents;

namespace Periscope.Data.Repositories
{
    public class TransactionLogRepository : RepositoryBase<CinderTransactionLog>, ITransactionLogRepository
    {
        public TransactionLogRepository(IMongoClient client, string databaseName) : base(client, databaseName,
            CollectionName.TransactionLogs) { }

        public Task UpsertAsync(FilterLogVO log)
        {
            return UpsertDocumentAsync(log.MapToStorageEntityForUpsert<CinderTransactionLog>());
        }

        public async Task<ITransactionLogView> FindByTransactionHashAndLogIndexAsync(string hash, BigInteger logIndex)
        {
            hash = hash.ToLowerInvariant();
            FilterDefinition<CinderTransactionLog> filter = CreateDocumentFilter(new CinderTransactionLog
            {
                TransactionHash = hash, LogIndex = logIndex.ToString()
            });
            CinderTransactionLog response = await Collection.Find(filter).SingleOrDefaultAsync().AnyContext();

            return response;
        }
    }
}
