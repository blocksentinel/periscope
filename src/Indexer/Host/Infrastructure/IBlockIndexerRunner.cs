using System.Threading;
using System.Threading.Tasks;

namespace Cinder.Indexer.Host.Infrastructure
{
    public interface IBlockIndexerRunner
    {
        Task Run(CancellationToken cancellationToken = default);
    }
}
