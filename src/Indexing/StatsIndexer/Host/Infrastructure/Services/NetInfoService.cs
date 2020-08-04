using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinder.Extensions;
using Foundatio.Caching;
using Nethereum.Web3;
using Newtonsoft.Json.Linq;

namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure.Services
{
    public class NetInfoService : INetInfoService
    {
        private const string TimestampCacheKey = "Timestamp";
        private const string NetworkHashRateCacheKey = "NetworkHashRate";
        private const string AverageBlockTimeCacheKey = "AverageBlockTime";
        private readonly ICacheClient _memoryCache;
        private readonly IWeb3 _web3;

        public NetInfoService(IWeb3 web3)
        {
            _web3 = web3;
            _memoryCache = new InMemoryCacheClient();
        }

        public async Task<decimal> GetAverageBlockTime(ulong timestamp)
        {
            decimal averageBlockTime = 0;
            ICollection<long> timestamps =
                await _memoryCache.GetAsync<ICollection<long>>(TimestampCacheKey, new List<long>()).AnyContext();

            timestamps.Add((long) timestamp);
            timestamps = timestamps.OrderBy(t => t).ToList();

            if (timestamps.Count > 50)
            {
                List<long> differences = new List<long>();
                int previousIndex = 0;
                for (int i = 0; i < timestamps.Count; i++)
                {
                    if (i == 0)
                    {
                        continue;
                    }

                    differences.Add(timestamps.ElementAt(i) - timestamps.ElementAt(previousIndex));
                    previousIndex = i;
                }

                averageBlockTime = (decimal) differences.Select(difference => difference).Average();
            }

            await _memoryCache.SetAsync(TimestampCacheKey, timestamps.Reverse().Take(5000).Reverse().ToList()).AnyContext();
            await _memoryCache.SetAsync(AverageBlockTimeCacheKey, averageBlockTime).AnyContext();

            return averageBlockTime;
        }

        public Task<decimal> GetDifficulty(ulong difficulty)
        {
            return difficulty == 0 ? Task.FromResult(0m) : Task.FromResult(difficulty / ((decimal) 1000 * 1000 * 1000));
        }

        public async Task<decimal> GetAverageNetworkHashRate(ulong difficulty)
        {
            if (difficulty == 0)
            {
                return 0;
            }

            decimal averageBlockTime = await _memoryCache.GetAsync<decimal>(AverageBlockTimeCacheKey, 0).AnyContext();

            if (averageBlockTime == 0)
            {
                return 0;
            }

            ICollection<long> networkHashRate =
                await _memoryCache.GetAsync<ICollection<long>>(NetworkHashRateCacheKey, new List<long>()).AnyContext();

            networkHashRate.Add((long) difficulty);

            decimal averageNetworkHashRate = (decimal) networkHashRate.Select(rate => rate).Average() /
                                             averageBlockTime /
                                             ((decimal) 1000 * 1000 * 1000);

            await _memoryCache.SetAsync(NetworkHashRateCacheKey, networkHashRate.Reverse().Take(5000).Reverse().ToList())
                .AnyContext();

            return averageNetworkHashRate;
        }

        public Task<int> GetConnectedPeerCount()
        {
            return Task.FromResult(0);
        }
    }
}
