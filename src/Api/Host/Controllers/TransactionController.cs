using System.Collections.Generic;
using System.Threading.Tasks;
using Cinder.Api.Application.Features.Transaction;
using Cinder.Core.Paging;
using Cinder.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinder.Api.Host.Controllers
{
    public class TransactionController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(typeof(GetTransactions.Model), StatusCodes.Status200OK)]
        public async Task<IPage<GetTransactions.Model>> GetTransactions(GetTransactions.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }

        [HttpGet("{hash}")]
        [ProducesResponseType(typeof(GetTransactionByHash.Model), StatusCodes.Status200OK)]
        public async Task<GetTransactionByHash.Model> GetTransactionByHash(GetTransactionByHash.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }

        [HttpGet("block/{hash}")]
        [ProducesResponseType(typeof(GetTransactionsByBlockHash.Model), StatusCodes.Status200OK)]
        public async Task<IEnumerable<GetTransactionsByBlockHash.Model>> GetTransactionsByBlockHash(
            GetTransactionsByBlockHash.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }

        [HttpGet("address/{hash}")]
        [ProducesResponseType(typeof(GetTransactionsByAddressHash.Model), StatusCodes.Status200OK)]
        public async Task<IPage<GetTransactionsByAddressHash.Model>> GetTransactionsByAddressHash(
            GetTransactionsByAddressHash.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }

        [HttpGet("address/{hash}/recent")]
        [ProducesResponseType(typeof(GetRecentTransactionsByAddressHash.Model), StatusCodes.Status200OK)]
        public async Task<IEnumerable<GetRecentTransactionsByAddressHash.Model>> GetRecentTransactionsByAddressHash(
            GetRecentTransactionsByAddressHash.Query query)
        {
            return await Mediator.Send(query).AnyContext();
        }
    }
}
