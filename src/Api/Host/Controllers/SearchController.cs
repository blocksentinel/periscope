using System.Threading.Tasks;
using Cinder.Api.Application.Features.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinder.Api.Host.Controllers
{
    public class SearchController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetResultsByQuery.Model), StatusCodes.Status200OK)]
        public Task<GetResultsByQuery.Model> GetResultsByQuery([FromQuery] string term)
        {
            return Mediator.Send(new GetResultsByQuery.Query {Term = term});
        }
    }
}
