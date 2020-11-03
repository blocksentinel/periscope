using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Periscope.Api.Application.Features.Stats;
using Periscope.Core.Paging;

namespace Periscope.Api.Host.Controllers
{
    public class StatsController : BaseController
    {
        [HttpGet("richest")]
        [ProducesResponseType(typeof(GetRichest.Model), StatusCodes.Status200OK)]
        public Task<IPage<GetRichest.Model>> GetRichest([FromQuery] int? page, [FromQuery] int? size)
        {
            return Mediator.Send(new GetRichest.Query {Page = page, Size = size});
        }

        [HttpGet("supply")]
        [ProducesResponseType(typeof(GetSupply.Model), StatusCodes.Status200OK)]
        public Task<GetSupply.Model> GetSupply()
        {
            return Mediator.Send(new GetSupply.Query());
        }

        [HttpGet("netinfo")]
        [ProducesResponseType(typeof(GetNetInfo.Model), StatusCodes.Status200OK)]
        public Task<GetNetInfo.Model> GetNetInfo()
        {
            return Mediator.Send(new GetNetInfo.Query());
        }

        [HttpGet("price")]
        [ProducesResponseType(typeof(GetPrice.Model), StatusCodes.Status200OK)]
        public Task<GetPrice.Model> GetPrice()
        {
            return Mediator.Send(new GetPrice.Query());
        }
    }
}
