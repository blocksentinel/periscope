using System.Threading;
using System.Threading.Tasks;
using Cinder.Documents;

namespace Cinder.Data.Repositories
{
    public interface IMinerRepository
    {
        Task<CinderMiner> GetByAddressOrDefault(string hash, CancellationToken cancellationToken = default);
    }
}
