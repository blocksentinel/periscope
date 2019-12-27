using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Documents;

namespace Cinder.Data.Repositories
{
    public interface IAddressMetaRepository
    {
        Task<CinderAddressMeta> GetByAddressOrDefault(string hash, CancellationToken cancellationToken = default);

        Task<IEnumerable<CinderAddressMeta>> GetByAddresses(IEnumerable<string> hashes,
            CancellationToken cancellationToken = default);
    }
}
