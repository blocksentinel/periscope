using Foundatio;
using Foundatio.Caching;

namespace Cinder.Stats
{
    public class StatsCache : RedisHybridCacheClient, IStatsCache
    {
        public StatsCache(RedisCacheClientOptions options, InMemoryCacheClientOptions localOptions = null) : base(options,
            localOptions) { }

        public StatsCache(Builder<RedisCacheClientOptionsBuilder, RedisCacheClientOptions> config,
            Builder<InMemoryCacheClientOptionsBuilder, InMemoryCacheClientOptions> localConfig = null) :
            base(config, localConfig) { }
    }
}
