namespace Cinder.Stats
{
    public class Price
    {
        public const string DefaultCacheKey = "Price";
        public decimal BtcPrice { get; set; }
        public decimal BtcMarketCap { get; set; }
        public decimal Btc24HrVol { get; set; }
        public decimal Btc24HrChange { get; set; }
        public decimal UsdPrice { get; set; }
        public decimal UsdMarketCap { get; set; }
        public decimal Usd24HrVol { get; set; }
        public decimal Usd24HrChange { get; set; }
    }
}
