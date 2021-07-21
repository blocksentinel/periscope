using System;
using System.Threading.Tasks;
using Foundatio.Caching;
using Foundatio.Jobs;
using Microsoft.Extensions.Logging;
using Periscope.Core.Data.Repositories;
using Periscope.Core.Extensions;
using Periscope.Core.SharedKernel;
using Periscope.Core.Stats;

namespace Periscope.Core.Jobs
{
    public class CirculatingSupplyJob : JobBase
    {
        private readonly IAddressRepository _addressRepository;
        private readonly ScopedHybridCacheClient _statsCache;

        public CirculatingSupplyJob(ILoggerFactory loggerFactory, IHybridCacheClient cacheClient,
            IAddressRepository addressRepository) : base(loggerFactory)
        {
            _addressRepository = addressRepository;
            _statsCache = new ScopedHybridCacheClient(cacheClient, CacheScopes.Stats);
        }

        protected override async Task<JobResult> RunInternalAsync(JobContext context)
        {
            try
            {
                CirculatingSupply supply = await _statsCache.GetAsync(CirculatingSupply.DefaultCacheKey, new CirculatingSupply())
                    .AnyContext();

                supply.Supply = await _addressRepository.GetSupply().AnyContext();

                await _statsCache.SetAsync(CirculatingSupply.DefaultCacheKey, supply).AnyContext();
            }
            catch (Exception e)
            {
                return JobResult.FromException(e);
            }

            return JobResult.Success;
        }
    }
}
