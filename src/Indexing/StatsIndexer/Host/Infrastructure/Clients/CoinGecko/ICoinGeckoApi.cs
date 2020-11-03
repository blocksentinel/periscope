using System.Threading;
using System.Threading.Tasks;
using Periscope.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko.Requests;
using Periscope.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko.Responses;
using Refit;

namespace Periscope.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko
{
    public interface ICoinGeckoApi
    {
        [Get("/v3/simple/price")]
        Task<SimplePriceResponse> GetSimplePrice([Query] SimplePriceRequest request,
            CancellationToken cancellationToken = default);
    }
}
