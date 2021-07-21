using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Periscope.Core.Documents;

namespace Periscope.Core.Data.Repositories
{
    public interface IAddressMetaRepository
    {
        Task<CinderAddressMeta> GetByAddressOrDefault(string hash, CancellationToken cancellationToken = default);

        Task<IEnumerable<CinderAddressMeta>> GetByAddresses(IEnumerable<string> hashes,
            CancellationToken cancellationToken = default);
    }
}
