using System.Threading;
using System.Threading.Tasks;
using Cinder.Extensions;
using Cinder.Stats;
using Foundatio.Caching;
using MediatR;

namespace Cinder.Api.Application.Features.Stats
{
    public class GetPrice
    {
        public class Query : IRequest<Model> { }

        public class Model
        {
            public decimal BtcPrice { get; set; }
            public decimal BtcMarketCap { get; set; }
            public decimal Btc24HrVol { get; set; }
            public decimal Btc24HrChange { get; set; }
            public decimal UsdPrice { get; set; }
            public decimal UsdMarketCap { get; set; }
            public decimal Usd24HrVol { get; set; }
            public decimal Usd24HrChange { get; set; }
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly IStatsCache _statsCache;

            public Handler(IStatsCache statsCache)
            {
                _statsCache = statsCache;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                Price price = await _statsCache.GetAsync<Price>(Price.DefaultCacheKey, null).AnyContext();

                if (price == null)
                {
                    return null;
                }

                return new Model
                {
                    BtcPrice = price.BtcPrice,
                    BtcMarketCap = price.BtcMarketCap,
                    Btc24HrVol = price.Btc24HrVol,
                    Btc24HrChange = price.Btc24HrChange,
                    UsdPrice = price.UsdPrice,
                    UsdMarketCap = price.UsdMarketCap,
                    Usd24HrVol = price.Usd24HrVol,
                    Usd24HrChange = price.Usd24HrChange
                };
            }
        }
    }
}
