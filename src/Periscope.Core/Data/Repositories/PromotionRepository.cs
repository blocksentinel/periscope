using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Periscope.Core.Documents;

namespace Periscope.Core.Data.Repositories
{
    public class PromotionRepository : RepositoryBase<Promotion>, IPromotionRepository
    {
        public PromotionRepository(IMongoClient client, string databaseName) : base(client, databaseName,
            CollectionName.Promotions) { }

        public async Task<IEnumerable<Promotion>> GetPromotions(bool includeInactive = false,
            CancellationToken cancellationToken = default)
        {
            if (includeInactive)
            {
                return await Collection.Find(FilterDefinition<Promotion>.Empty).ToListAsync(cancellationToken);
            }

            return await Collection.Find(promotion => promotion.Active).ToListAsync(cancellationToken);
        }
    }
}
