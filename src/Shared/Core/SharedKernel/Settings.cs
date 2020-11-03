namespace Periscope.Core.SharedKernel
{
    public class Settings : ISettings
    {
        public IBusSettings Bus { get; set; } = new BusSettings();
        public IDatabaseSettings Database { get; set; } = new DatabaseSettings();
        public INodeSettings Node { get; set; } = new NodeSettings();
        public IRedisSettings Redis { get; set; } = new RedisSettings();
        public IPerformanceSettings Performance { get; set; } = new PerformanceSettings();

        public class BusSettings : IBusSettings
        {
            public string ConnectionString { get; set; }
            public string QueueName { get; set; }
        }

        public class DatabaseSettings : IDatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string Tag { get; set; }
            public string Locale { get; set; }
        }

        public class NodeSettings : INodeSettings
        {
            public string RpcUrl { get; set; }
        }

        public class RedisSettings : IRedisSettings
        {
            public string ConnectionString { get; set; }
        }

        public class PerformanceSettings : IPerformanceSettings
        {
            public int QueryCountLimiter { get; set; }
            public int RichListMinimumBalance { get; set; }
        }
    }
}
