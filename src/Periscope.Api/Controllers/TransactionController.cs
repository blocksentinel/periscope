using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Periscope.Core.Data.Repositories;
using Periscope.Core.Features.Transaction;
using Periscope.Core.Paging;

namespace Periscope.Api.Controllers
{
    public class TransactionController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetTransactions.Model), StatusCodes.Status200OK)]
        public Task<IPage<GetTransactions.Model>> GetTransactions([FromQuery] int? page, [FromQuery] int? size,
            [FromQuery] SortOrder sort = SortOrder.Ascending)
        {
            return Mediator.Send(new GetTransactions.Query {Page = page, Size = size, Sort = sort});
        }

        [HttpGet("{hash}")]
        [ProducesResponseType(typeof(GetTransactionByHash.Model), StatusCodes.Status200OK)]
        public Task<GetTransactionByHash.Model> GetTransactionByHash([FromRoute] string hash)
        {
            return Mediator.Send(new GetTransactionByHash.Query {Hash = hash});
        }

        [HttpGet("block/{hash}")]
        [ProducesResponseType(typeof(GetTransactionsByBlockHash.Model), StatusCodes.Status200OK)]
        public Task<IEnumerable<GetTransactionsByBlockHash.Model>> GetTransactionsByBlockHash([FromRoute] string hash)
        {
            return Mediator.Send(new GetTransactionsByBlockHash.Query {BlockHash = hash});
        }

        [HttpGet("address/{hash}")]
        [ProducesResponseType(typeof(GetTransactionsByAddressHash.Model), StatusCodes.Status200OK)]
        public Task<IPage<GetTransactionsByAddressHash.Model>> GetTransactionsByAddressHash([FromRoute] string hash,
            [FromQuery] int? page, [FromQuery] int? size, [FromQuery] SortOrder sort = SortOrder.Ascending)
        {
            return Mediator.Send(
                new GetTransactionsByAddressHash.Query {AddressHash = hash, Page = page, Size = size, Sort = sort});
        }

        [HttpGet("address/{hash}/recent")]
        [ProducesResponseType(typeof(GetRecentTransactionsByAddressHash.Model), StatusCodes.Status200OK)]
        public Task<IEnumerable<GetRecentTransactionsByAddressHash.Model>> GetRecentTransactionsByAddressHash(
            [FromRoute] string hash, [FromQuery] int? size)
        {
            return Mediator.Send(new GetRecentTransactionsByAddressHash.Query {AddressHash = hash, Size = size});
        }
    }
}
