using System.Threading;
using System.Threading.Tasks;
using Periscope.Core.Clients.CoinGecko.Requests;
using Periscope.Core.Clients.CoinGecko.Responses;
using Refit;

namespace Periscope.Core.Clients.CoinGecko
{
    public interface ICoinGeckoApi
    {
        [Get("/v3/simple/price")]
        Task<SimplePriceResponse> GetSimplePrice([Query] SimplePriceRequest request,
            CancellationToken cancellationToken = default);
    }
}
