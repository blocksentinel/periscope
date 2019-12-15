using System.Threading.Tasks;
using Cinder.Api.Application.Features.Block;
using Cinder.Core.Paging;
using Cinder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinder.Api.Host.Controllers
{
    public class BlockController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetBlocks.Model), StatusCodes.Status200OK)]
        public async Task<IPage<GetBlocks.Model>> GetBlocks(GetBlocks.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }

        [HttpGet("{hash}")]
        [ProducesResponseType(typeof(GetBlockByHash.Model), StatusCodes.Status200OK)]
        public async Task<GetBlockByHash.Model> GetBlockByHash(GetBlockByHash.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }

        [HttpGet("height/{number}")]
        [ProducesResponseType(typeof(GetBlockByNumber.Model), StatusCodes.Status200OK)]
        public async Task<GetBlockByNumber.Model> GetBlockByNumber(GetBlockByNumber.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }
    }
}
