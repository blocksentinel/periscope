using Cinder.Indexing.HostBase;

namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure
{
    public class IndexerSettings : SettingsBase
    {
        public RedisSettings Redis { get; set; } = new RedisSettings();

        public class RedisSettings
        {
            public string ConnectionString { get; set; }
        }
    }
}
