using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Documents;

namespace Cinder.Data.Repositories
{
    public interface IBlockRepository : Nethereum.BlockchainProcessing.BlockStorage.Repositories.IBlockRepository
    {
        Task<IPage<CinderBlock>> GetBlocks(int? page = null, int? size = null, SortOrder sort = SortOrder.Ascending,
            CancellationToken cancellationToken = default);

        Task<CinderBlock> GetBlockByHash(string hash, CancellationToken cancellationToken = default);
        Task<string> GetBlockHashIfExists(string hash, CancellationToken cancellationToken = default);
        Task<CinderBlock> GetBlockByNumber(ulong number, CancellationToken cancellationToken = default);
        Task<string> GetBlockNumberIfExists(ulong number, CancellationToken cancellationToken = default);
        Task<ulong> GetBlocksMinedCountByAddressHash(string addressHash, CancellationToken cancellationToken = default);
    }
}
