namespace Cinder.Stats
{
    public class NetInfo
    {
        public const string DefaultCacheKey = "NetInfo";
        public ulong BestBlock { get; set; }
        public ulong BestBlockTimestamp { get; set; }
        public decimal AverageBlockTime { get; set; }
        public decimal AverageNetworkHashRate { get; set; }
        public decimal Difficulty { get; set; }
    }
}
