using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Periscope.Core.Features.Search;

namespace Periscope.Api.Controllers
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
