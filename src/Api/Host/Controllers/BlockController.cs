using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Periscope.Api.Application.Features.Block;
using Periscope.Core.Paging;
using Periscope.Data.Repositories;

namespace Periscope.Api.Host.Controllers
{
    public class BlockController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetBlocks.Model), StatusCodes.Status200OK)]
        public Task<IPage<GetBlocks.Model>> GetBlocks([FromQuery] int? page, [FromQuery] int? size,
            [FromQuery] SortOrder sort = SortOrder.Ascending)
        {
            return Mediator.Send(new GetBlocks.Query {Page = page, Size = size, Sort = sort});
        }

        [HttpGet("{hash}")]
        [ProducesResponseType(typeof(GetBlockByHash.Model), StatusCodes.Status200OK)]
        public Task<GetBlockByHash.Model> GetBlockByHash([FromRoute] string hash)
        {
            return Mediator.Send(new GetBlockByHash.Query {Hash = hash});
        }

        [HttpGet("height/{number}")]
        [ProducesResponseType(typeof(GetBlockByNumber.Model), StatusCodes.Status200OK)]
        public Task<GetBlockByNumber.Model> GetBlockByNumber([FromRoute] ulong number)
        {
            return Mediator.Send(new GetBlockByNumber.Query {Number = number});
        }
    }
}
