using Cinder.Core.SharedKernel;

namespace Cinder.Api.Host.Infrastructure
{
    public class Settings
    {
        public DatabaseSettings Database { get; set; } = new DatabaseSettings();
        public NodeSettings Node { get; set; } = new NodeSettings();
        public QueueSettings Queue { get; set; } = new QueueSettings();

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

        public class QueueSettings
        {
            public string ConnectionString { get; set; }
            public string Name { get; set; }
        }
    }
}
