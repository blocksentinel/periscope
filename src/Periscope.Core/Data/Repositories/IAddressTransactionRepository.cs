using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Periscope.Core.Paging;

namespace Periscope.Core.Data.Repositories
{
    public interface
        IAddressTransactionRepository : Nethereum.BlockchainProcessing.BlockStorage.Repositories.IAddressTransactionRepository
    {
        Task<IEnumerable<string>> GetUniqueAddresses(CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetTransactionHashesByAddressHash(string addressHash, int? size = null,
            CancellationToken cancellationToken = default);

        Task<IPage<string>> GetPagedTransactionHashesByAddressHash(string addressHash, int? page = null, int? size = null,
            SortOrder sort = SortOrder.Ascending, int queryCountLimiter = 10000, CancellationToken cancellationToken = default);

        Task<ulong> GetTransactionCountByAddressHash(string addressHash, int queryCountLimiter = 10000,
            CancellationToken cancellationToken = default);
    }
}
