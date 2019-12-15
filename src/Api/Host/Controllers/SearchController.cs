using System.Threading.Tasks;
using Cinder.Api.Application.Features.Search;
using Cinder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinder.Api.Host.Controllers
{
    public class SearchController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetResultsByQuery.Model), StatusCodes.Status200OK)]
        public async Task<GetResultsByQuery.Model> GetResultsByQuery(GetResultsByQuery.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }
    }
}
