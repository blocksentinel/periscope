using Refit;

namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko.Requests
{
    public class SimplePriceRequest
    {
        public string[] Ids { private get; set; }

        [AliasAs("ids")]
        public string IdsStr => string.Join(",", Ids);

        public string[] Currencies { private get; set; }

        [AliasAs("vs_currencies")]
        public string CurrenciesStr => string.Join(",", Currencies);

        public bool IncludeMarketCap { get; set; }

        [AliasAs("include_market_cap")]
        public string IncludeMarketCapStr => IncludeMarketCap.ToString().ToLowerInvariant();

        public bool Include24HrVol { get; set; }

        [AliasAs("include_24hr_vol")]
        public string Include24HrVolStr => Include24HrVol.ToString().ToLowerInvariant();

        public bool Include24HrChange { get; set; }

        [AliasAs("include_24hr_change")]
        public string Include24HrChangeStr => Include24HrChange.ToString().ToLowerInvariant();

        public bool IncludeLastUpdatedAt { get; set; }

        [AliasAs("include_last_updated_at")]
        public string IncludeLastUpdatedAtStr => IncludeLastUpdatedAt.ToString().ToLowerInvariant();
    }
}
