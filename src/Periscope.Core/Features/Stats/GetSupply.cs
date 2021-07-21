using System.Threading;
using System.Threading.Tasks;
using Foundatio.Caching;
using MediatR;
using Periscope.Core.Extensions;
using Periscope.Core.SharedKernel;
using Periscope.Core.Stats;

namespace Periscope.Core.Features.Stats
{
    public class GetSupply
    {
        public class Query : IRequest<Model> { }

        public class Model
        {
            public decimal Supply { get; set; }
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
                CirculatingSupply supply = await _statsCache.GetAsync(CirculatingSupply.DefaultCacheKey, new CirculatingSupply())
                    .AnyContext();

                return new Model {Supply = supply.Supply};
            }
        }
    }
}
