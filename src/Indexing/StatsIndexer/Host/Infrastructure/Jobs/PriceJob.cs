using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cinder.Extensions;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko.Requests;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko.Responses;
using Cinder.Stats;
using Foundatio.Caching;
using Foundatio.Jobs;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure.Jobs
{
    public class PriceJob : JobBase, IDisposable
    {
        private static readonly string[] Ids = {Ellaism.Id};
        private static readonly string[] Currencies = {Ellaism.Btc, Ellaism.Usd};
        private readonly ICoinGeckoApi _api;
        private readonly StatsCache _statsCache;

        public PriceJob(ILoggerFactory loggerFactory, IConnectionMultiplexer muxer, ICoinGeckoApi api) : base(loggerFactory)
        {
            _api = api;
            _statsCache =
                new StatsCache(new RedisCacheClientOptions {ConnectionMultiplexer = muxer, LoggerFactory = loggerFactory});
        }

        public void Dispose()
        {
            _statsCache?.Dispose();
        }

        protected override async Task<JobResult> RunInternalAsync(JobContext context)
        {
            try
            {
                SimplePriceResponse response = await _api.GetSimplePrice(new SimplePriceRequest
                    {
                        Ids = Ids,
                        Currencies = Currencies,
                        IncludeMarketCap = true,
                        Include24HrChange = true,
                        Include24HrVol = true
                    })
                    .AnyContext();

                Price price = await _statsCache.GetAsync(Price.DefaultCacheKey, new Price()).AnyContext();

                price.BtcPrice = GetCoinPropertyByKey(response, Ellaism.Id, Ellaism.Btc);
                price.BtcMarketCap = GetCoinPropertyByKey(response, Ellaism.Id, Ellaism.BtcMarketCap);
                price.Btc24HrVol = GetCoinPropertyByKey(response, Ellaism.Id, Ellaism.Btc24HrVol);
                price.Btc24HrChange = GetCoinPropertyByKey(response, Ellaism.Id, Ellaism.Btc24HrChange);
                price.UsdPrice = GetCoinPropertyByKey(response, Ellaism.Id, Ellaism.Usd);
                price.UsdMarketCap = GetCoinPropertyByKey(response, Ellaism.Id, Ellaism.UsdMarketCap);
                price.Usd24HrVol = GetCoinPropertyByKey(response, Ellaism.Id, Ellaism.Usd24HrVol);
                price.Usd24HrChange = GetCoinPropertyByKey(response, Ellaism.Id, Ellaism.Usd24HrChange);

                await _statsCache.AddAsync(Price.DefaultCacheKey, price).AnyContext();
            }
            catch (Exception e)
            {
                return JobResult.FromException(e);
            }

            return JobResult.Success;
        }

        private static decimal GetCoinPropertyByKey(SimplePriceResponse response, string coin, string property)
        {
            Dictionary<string, double?> t = response.FirstOrDefault(r => r.Key == coin).Value;

            double? value = t?.FirstOrDefault(p => p.Key == property).Value;

            return value != null ? (decimal) value : 0;
        }

        internal class Ellaism
        {
            private const string UsdPrefix = "usd";
            private const string BtcPrefix = "btc";
            public const string Id = "ellaism";
            public const string Btc = BtcPrefix;
            public const string BtcMarketCap = BtcPrefix + "_market_cap";
            public const string Btc24HrVol = BtcPrefix + "_24h_vol";
            public const string Btc24HrChange = BtcPrefix + "_24h_change";
            public const string Usd = UsdPrefix;
            public const string UsdMarketCap = UsdPrefix + "_market_cap";
            public const string Usd24HrVol = UsdPrefix + "_24h_vol";
            public const string Usd24HrChange = UsdPrefix + "_24h_change";
        }
    }
}
