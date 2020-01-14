using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.SharedKernel;
using Cinder.Extensions;
using Cinder.Stats;
using Foundatio.Caching;
using MediatR;

namespace Cinder.Api.Application.Features.Stats
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
