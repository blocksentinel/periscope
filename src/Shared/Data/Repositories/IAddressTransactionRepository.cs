using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;

namespace Cinder.Data.Repositories
{
    public interface
        IAddressTransactionRepository : Nethereum.BlockchainProcessing.BlockStorage.Repositories.IAddressTransactionRepository
    {
        Task<IEnumerable<string>> GetUniqueAddresses(CancellationToken cancellationToken = default);

        Task<IEnumerable<string>> GetTransactionHashesByAddressHash(string addressHash, int? size = null,
            CancellationToken cancellationToken = default);

        Task<IPage<string>> GetPagedTransactionHashesByAddressHash(string addressHash, int? page = null, int? size = null,
            SortOrder sort = SortOrder.Ascending, CancellationToken cancellationToken = default);

        Task<ulong> GetTransactionCountByAddressHash(string addressHash, CancellationToken cancellationToken = default);
    }
}
