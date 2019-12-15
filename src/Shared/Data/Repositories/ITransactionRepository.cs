using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Documents;

namespace Cinder.Data.Repositories
{
    public interface ITransactionRepository : Nethereum.BlockchainProcessing.BlockStorage.Repositories.ITransactionRepository
    {
        Task<IPage<CinderTransaction>> GetTransactions(int? page = null, int? size = null, SortOrder sort = SortOrder.Ascending,
            CancellationToken cancellationToken = default);

        Task<CinderTransaction> GetTransactionByHash(string hash, CancellationToken cancellationToken = default);

        Task<IEnumerable<CinderTransaction>> GetTransactionsByBlockHash(string blockHash,
            CancellationToken cancellationToken = default);

        Task<string> GetTransactionHashIfExists(string hash, CancellationToken cancellationToken = default);

        Task<IEnumerable<CinderTransaction>> GetTransactionsByHashes(IEnumerable<string> transactionHashes,
            CancellationToken cancellationToken = default);
    }
}
