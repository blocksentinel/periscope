using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Periscope.Core.Documents;
using Periscope.Core.Paging;

namespace Periscope.Core.Data.Repositories
{
    public interface IAddressRepository : IRepository
    {
        Task UpsertAddress(CinderAddress address, CancellationToken cancellationToken = default);
        Task BulkUpsertAddresses(IEnumerable<CinderAddress> addresses, CancellationToken cancellationToken = default);
        Task<CinderAddress> GetAddressByHash(string hash, CancellationToken cancellationToken = default);
        Task<string> GetAddressHashIfExists(string hash, CancellationToken cancellationToken = default);

        Task<IEnumerable<CinderAddress>> GetStaleAddresses(int age = 5, int limit = 1000,
            CancellationToken cancellationToken = default);

        Task<IPage<CinderAddress>> GetRichest(int? page, int? size, int minimumBalance = 0, int queryCountLimiter = 10000,
            CancellationToken cancellationToken = default);

        Task<decimal> GetSupply(CancellationToken cancellationToken = default);
        Task<bool> AddressExists(string hash, CancellationToken cancellationToken = default);
    }
}
