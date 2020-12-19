using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Foundatio.Caching;
using MediatR;
using Periscope.Core.Extensions;
using Periscope.Core.SharedKernel;
using Periscope.Data.Repositories;
using Periscope.Documents;
using Periscope.Stats;

namespace Periscope.Api.Application.Features.Stats
{
    public class GetNetInfo
    {
        public class Query : IRequest<Model> { }

        public class Model
        {
            public ulong BestBlock { get; set; }
            public ulong BestBlockTimestamp { get; set; }
            public decimal AverageBlockTime { get; set; }
            public decimal AverageNetworkHashRate { get; set; }
            public decimal Difficulty { get; set; }
            public int ConnectedPeerCount { get; set; }
            public IEnumerable<PromotionModel> Promotions { get; set; }

            public class PromotionModel
            {
                public string Display { get; set; }
                public string Url { get; set; }
                public string Location { get; set; }
            }
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly IPromotionRepository _promotionRepository;
            private readonly ScopedHybridCacheClient _statsCache;

            public Handler(IPromotionRepository promotionRepository, IHybridCacheClient cacheClient)
            {
                _promotionRepository = promotionRepository;
                _statsCache = new ScopedHybridCacheClient(cacheClient, CacheScopes.Stats);
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                NetInfo netInfo = await _statsCache.GetAsync<NetInfo>(NetInfo.DefaultCacheKey, null).AnyContext();

                if (netInfo == null)
                {
                    return null;
                }

                IEnumerable<Promotion> promotions =
                    await _promotionRepository.GetPromotions(cancellationToken: cancellationToken);

                return new Model
                {
                    BestBlock = netInfo.BestBlock,
                    BestBlockTimestamp = netInfo.BestBlockTimestamp,
                    AverageBlockTime = netInfo.AverageBlockTime,
                    AverageNetworkHashRate = netInfo.AverageNetworkHashRate,
                    Difficulty = netInfo.Difficulty,
                    ConnectedPeerCount = netInfo.ConnectedPeerCount,
                    Promotions = promotions.Select(promotion => new Model.PromotionModel
                    {
                        Display = promotion.Display, Url = promotion.Url, Location = promotion.Location
                    })
                };
            }
        }
    }
}
