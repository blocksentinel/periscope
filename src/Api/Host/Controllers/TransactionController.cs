using System.Collections.Generic;
using System.Threading.Tasks;
using Cinder.Api.Application.Features.Transaction;
using Cinder.Core.Paging;
using Cinder.Data.Repositories;
using Cinder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinder.Api.Host.Controllers
{
    public class TransactionController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetTransactions.Model), StatusCodes.Status200OK)]
        public async Task<IPage<GetTransactions.Model>> GetTransactions([FromQuery] int? page, [FromQuery] int? size,
            [FromQuery] SortOrder sort = SortOrder.Ascending)
        {
            return await Mediator.Send(new GetTransactions.Query {Page = page, Size = size, Sort = sort}).AnyContext();
        }

        [HttpGet("{hash}")]
        [ProducesResponseType(typeof(GetTransactionByHash.Model), StatusCodes.Status200OK)]
        public async Task<GetTransactionByHash.Model> GetTransactionByHash([FromRoute] string hash)
        {
            return await Mediator.Send(new GetTransactionByHash.Query {Hash = hash}).AnyContext();
        }

        [HttpGet("block/{hash}")]
        [ProducesResponseType(typeof(GetTransactionsByBlockHash.Model), StatusCodes.Status200OK)]
        public async Task<IEnumerable<GetTransactionsByBlockHash.Model>> GetTransactionsByBlockHash([FromRoute] string hash)
        {
            return await Mediator.Send(new GetTransactionsByBlockHash.Query {BlockHash = hash}).AnyContext();
        }

        [HttpGet("address/{hash}")]
        [ProducesResponseType(typeof(GetTransactionsByAddressHash.Model), StatusCodes.Status200OK)]
        public async Task<IPage<GetTransactionsByAddressHash.Model>> GetTransactionsByAddressHash([FromRoute] string hash,
            [FromQuery] int? page, [FromQuery] int? size, [FromQuery] SortOrder sort = SortOrder.Ascending)
        {
            return await Mediator
                .Send(new GetTransactionsByAddressHash.Query {AddressHash = hash, Page = page, Size = size, Sort = sort})
                .AnyContext();
        }

        [HttpGet("address/{hash}/recent")]
        [ProducesResponseType(typeof(GetRecentTransactionsByAddressHash.Model), StatusCodes.Status200OK)]
        public async Task<IEnumerable<GetRecentTransactionsByAddressHash.Model>> GetRecentTransactionsByAddressHash(
            [FromRoute] string hash, [FromQuery] int? size)
        {
            return await Mediator.Send(new GetRecentTransactionsByAddressHash.Query {AddressHash = hash, Size = size})
                .AnyContext();
        }
    }
}
