using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Documents;

namespace Cinder.Data.Repositories
{
    public interface IAddressRepository : IRepository
    {
        Task UpsertAddress(CinderAddress address, CancellationToken cancellationToken = default);
        Task BulkUpsertAddresses(IEnumerable<CinderAddress> addresses, CancellationToken cancellationToken = default);
        Task<CinderAddress> GetAddressByHash(string hash, CancellationToken cancellationToken = default);

        Task<IEnumerable<CinderAddress>> GetStaleAddresses(int age = 5, int limit = 1000,
            CancellationToken cancellationToken = default);

        Task<IPage<CinderAddress>> GetRichest(int? page, int? size, CancellationToken cancellationToken = default);
    }
}
