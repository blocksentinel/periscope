using System.Threading.Tasks;

namespace Periscope.Core.Services
{
    public interface INetInfoService
    {
        Task<decimal> GetAverageBlockTime(ulong timestamp);
        Task<decimal> GetDifficulty(ulong difficulty);
        Task<decimal> GetAverageNetworkHashRate(ulong difficulty);
        Task<int> GetConnectedPeerCount();
    }
}
