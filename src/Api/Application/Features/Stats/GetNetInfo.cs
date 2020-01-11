using System.Threading;
using System.Threading.Tasks;
using Cinder.Extensions;
using Cinder.Stats;
using Foundatio.Caching;
using MediatR;

namespace Cinder.Api.Application.Features.Stats
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
                NetInfo netInfo = await _statsCache.GetAsync<NetInfo>(NetInfo.DefaultCacheKey, null).AnyContext();

                if (netInfo == null)
                {
                    return null;
                }

                return new Model
                {
                    BestBlock = netInfo.BestBlock,
                    BestBlockTimestamp = netInfo.BestBlockTimestamp,
                    AverageBlockTime = netInfo.AverageBlockTime,
                    AverageNetworkHashRate = netInfo.AverageNetworkHashRate,
                    Difficulty = netInfo.Difficulty
                };
            }
        }
    }
}
