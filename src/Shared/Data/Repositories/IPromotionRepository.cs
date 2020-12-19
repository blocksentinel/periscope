using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Periscope.Documents;

namespace Periscope.Data.Repositories
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> GetPromotions(bool includeInactive = false, CancellationToken cancellationToken = default);
    }
}
