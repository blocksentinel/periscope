using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Foundatio.Caching;
using MediatR;
using Microsoft.Extensions.Options;
using Periscope.Core.Extensions;
using Periscope.Core.Paging;
using Periscope.Core.SharedKernel;
using Periscope.Data.Repositories;
using Periscope.Documents;
using Periscope.Stats;

namespace Periscope.Api.Application.Features.Stats
{
    public class GetRichest
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Page).GreaterThanOrEqualTo(1);
                RuleFor(m => m.Size).InclusiveBetween(1, 100);
            }
        }

        public class Query : IRequest<IPage<Model>>
        {
            public int? Page { get; set; }
            public int? Size { get; set; }
        }

        public class Model
        {
            public int Rank { get; set; }
            public string Name { get; set; }
            public string Hash { get; set; }
            public decimal Balance { get; set; }
            public decimal? Percent { get; set; }
            public ICollection<string> Tags { get; set; } = new List<string>();
            public IDictionary<string, decimal> BalanceHistory { get; set; } = new Dictionary<string, decimal>();
        }

        public class Handler : IRequestHandler<Query, IPage<Model>>
        {
            private readonly IAddressMetaRepository _addressMetaRepository;
            private readonly IAddressRepository _addressRepository;
            private readonly Settings _settings;
            private readonly ScopedHybridCacheClient _statsCache;

            public Handler(IAddressRepository addressRepository, IAddressMetaRepository addressMetaRepository,
                IHybridCacheClient cacheClient, IOptions<Settings> options)
            {
                _addressRepository = addressRepository;
                _addressMetaRepository = addressMetaRepository;
                _statsCache = new ScopedHybridCacheClient(cacheClient, CacheScopes.Stats);
                _settings = options.Value;
            }

            public async Task<IPage<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                CirculatingSupply supply = await _statsCache.GetAsync<CirculatingSupply>(CirculatingSupply.DefaultCacheKey, null)
                    .AnyContext();
                IPage<CinderAddress> page = await _addressRepository.GetRichest(request.Page, request.Size,
                        _settings.Performance.RichListMinimumBalance, _settings.Performance.QueryCountLimiter, cancellationToken)
                    .AnyContext();
                IEnumerable<CinderAddressMeta> metas = await _addressMetaRepository
                    .GetByAddresses(page.Items.Select(address => address.Hash).Distinct(), cancellationToken)
                    .AnyContext();

                decimal circulatingSupply = supply?.Supply ?? 0;
                int rank = 1 * (page.Page - 1) * page.Size + 1;
                IEnumerable<Model> models = page.Items.Select(address =>
                {
                    CinderAddressMeta meta = metas.FirstOrDefault(x => x.Id == address.Id);

                    return new Model
                    {
                        Rank = rank++,
                        Name = meta?.Name,
                        Hash = address.Hash,
                        Balance = address.Balance,
                        Percent = address.Balance > 0 && circulatingSupply > 0
                            ? address.Balance / circulatingSupply * 100
                            : default,
                        Tags = meta?.Tags ?? new List<string>(),
                        BalanceHistory = address.BalanceHistory ?? new Dictionary<string, decimal>()
                    };
                });

                return new PagedEnumerable<Model>(models, page.Total, page.Page, page.Size);
            }
        }
    }
}
