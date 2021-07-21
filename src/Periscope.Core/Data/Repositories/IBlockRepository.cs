using System.Threading;
using System.Threading.Tasks;
using Periscope.Core.Documents;
using Periscope.Core.Paging;

namespace Periscope.Core.Data.Repositories
{
    public interface IBlockRepository : Nethereum.BlockchainProcessing.BlockStorage.Repositories.IBlockRepository
    {
        Task<IPage<CinderBlock>> GetBlocks(int? page = null, int? size = null, SortOrder sort = SortOrder.Ascending,
            CancellationToken cancellationToken = default);

        Task<CinderBlock> GetBlockByHash(string hash, CancellationToken cancellationToken = default);
        Task<string> GetBlockHashIfExists(string hash, CancellationToken cancellationToken = default);
        Task<CinderBlock> GetBlockByNumber(ulong number, CancellationToken cancellationToken = default);
        Task<string> GetBlockHashByBlockNumberIfExists(ulong number, CancellationToken cancellationToken = default);
        Task<ulong> GetBlocksMinedCountByAddressHash(string addressHash, CancellationToken cancellationToken = default);
    }
}
