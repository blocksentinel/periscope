using System.Threading;
using System.Threading.Tasks;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko.Requests;
using Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko.Responses;
using Refit;

namespace Cinder.Indexing.StatsIndexer.Host.Infrastructure.Clients.CoinGecko
{
    public interface ICoinGeckoApi
    {
        [Get("/v3/simple/price")]
        Task<SimplePriceResponse> GetSimplePrice([Query] SimplePriceRequest request,
            CancellationToken cancellationToken = default);
    }
}
