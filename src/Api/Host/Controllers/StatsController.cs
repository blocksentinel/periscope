using System.Threading.Tasks;
using Cinder.Api.Application.Features.Stats;
using Cinder.Core.Paging;
using Cinder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinder.Api.Host.Controllers
{
    public class StatsController : BaseController
    {
        [HttpGet("richest")]
        [ProducesResponseType(typeof(GetRichest.Model), StatusCodes.Status200OK)]
        public async Task<IPage<GetRichest.Model>> GetRichest([FromQuery] int? page, [FromQuery] int? size)
        {
            return await Mediator.Send(new GetRichest.Query {Page = page, Size = size}).AnyContext();
        }
    }
}
