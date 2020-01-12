using System;
using System.Threading.Tasks;
using Cinder.Extensions;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Services;
using Cinder.Stats;
using Foundatio.Caching;
using Foundatio.Jobs;
using Foundatio.Queues;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure.Jobs
{
    public class NetInfoJob : QueueJobBase<NetInfoWorkItem>, IDisposable
    {
        private readonly NetInfoService _netInfoService;
        private readonly ICacheClient _statsCache;

        public NetInfoJob(IQueue<NetInfoWorkItem> queue, ILoggerFactory loggerFactory, NetInfoService netInfoService,
            IConnectionMultiplexer muxer) : base(queue, loggerFactory)
        {
            _netInfoService = netInfoService;
            _statsCache =
                new StatsCache(new RedisCacheClientOptions {ConnectionMultiplexer = muxer, LoggerFactory = loggerFactory});
        }

        public void Dispose()
        {
            _statsCache?.Dispose();
        }

        protected override async Task<JobResult> ProcessQueueEntryAsync(QueueEntryContext<NetInfoWorkItem> context)
        {
            try
            {
                NetInfoWorkItem block = context.QueueEntry.Value;
                _logger.LogDebug("NetStatsJob fired, Bock: {@Block}", block);

                NetInfo netInfo = await _statsCache.GetAsync(NetInfo.DefaultCacheKey, new NetInfo()).AnyContext();

                netInfo.BestBlock = block.BlockNumber;
                netInfo.BestBlockTimestamp = block.Timestamp;
                netInfo.Difficulty = await _netInfoService.GetDifficulty(block.Difficulty).AnyContext();
                netInfo.AverageBlockTime = await _netInfoService.GetAverageBlockTime(block.Timestamp).AnyContext();
                netInfo.AverageNetworkHashRate = await _netInfoService.GetAverageNetworkHashRate(block.Difficulty).AnyContext();

                await _statsCache.SetAsync(NetInfo.DefaultCacheKey, netInfo).AnyContext();
            }
            catch (Exception e)
            {
                return JobResult.FromException(e);
            }

            return JobResult.Success;
        }
    }
}
