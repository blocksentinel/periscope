using Cinder.Core.SharedKernel;

namespace Cinder.Indexers.HostBase
{
    public class SettingsBase
    {
        public DatabaseSettings Database { get; set; } = new DatabaseSettings();
        public NodeSettings Node { get; set; } = new NodeSettings();
        public BusSettings Bus { get; set; } = new BusSettings();

        public class DatabaseSettings : IDatabaseSettings
        {
            public string ConnectionString { get; set; }
            public string Tag { get; set; }
            public string Locale { get; set; }
        }

        public class NodeSettings
        {
            public string RpcUrl { get; set; }
        }

        public class BusSettings
        {
            public string ConnectionString { get; set; }
            public string QueueName { get; set; }
        }
    }
}
