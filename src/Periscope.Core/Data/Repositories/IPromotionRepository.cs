using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Periscope.Core.Documents;

namespace Periscope.Core.Data.Repositories
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<Promotion>> GetPromotions(bool includeInactive = false, CancellationToken cancellationToken = default);
    }
}
