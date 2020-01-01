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
    public class NetStatsJob : QueueJobBase<NetStatsWorkItem>, IDisposable
    {
        private readonly NetInfoService _netInfoService;
        private readonly ICacheClient _statsCache;

        public NetStatsJob(IQueue<NetStatsWorkItem> queue, ILoggerFactory loggerFactory, NetInfoService netInfoService,
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

        protected override async Task<JobResult> ProcessQueueEntryAsync(QueueEntryContext<NetStatsWorkItem> context)
        {
            try
            {
                NetStatsWorkItem block = context.QueueEntry.Value;
                _logger.LogDebug("NetStatsJob fired, Bock: {@Block}", block);

                NetInfo netInfo = new NetInfo
                {
                    BestBlock = block.BlockNumber,
                    BestBlockTimestamp = block.Timestamp,
                    Difficulty = await _netInfoService.GetDifficulty(block.Difficulty).AnyContext(),
                    AverageBlockTime = await _netInfoService.GetAverageBlockTime(block.Timestamp).AnyContext(),
                    AverageNetworkHashRate = await _netInfoService.GetAverageNetworkHashRate(block.Difficulty).AnyContext()
                };

                await _statsCache.AddAsync(NetInfo.DefaultCacheKey, netInfo).AnyContext();
            }
            catch (Exception e)
            {
                return JobResult.FromException(e);
            }

            return JobResult.Success;
        }
    }
}
