using System.Threading;
using System.Threading.Tasks;
using Foundatio.Caching;
using MediatR;
using Periscope.Core.Extensions;
using Periscope.Core.SharedKernel;
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
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly ScopedHybridCacheClient _statsCache;

            public Handler(IHybridCacheClient cacheClient)
            {
                _statsCache = new ScopedHybridCacheClient(cacheClient, CacheScopes.Stats);
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
                    Difficulty = netInfo.Difficulty,
                    ConnectedPeerCount = netInfo.ConnectedPeerCount
                };
            }
        }
    }
}
