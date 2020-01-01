namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure.Jobs
{
    public class NetStatsWorkItem
    {
        public ulong BlockNumber { get; set; }
        public ulong Difficulty { get; set; }
        public ulong Timestamp { get; set; }
        public ulong UncleCount { get; set; }
        public ulong TransactionCount { get; set; }
    }
}
