using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Periscope.Api.Application.Features.Address;

namespace Periscope.Api.Host.Controllers
{
    public class AddressController : BaseController
    {
        [HttpGet("{hash}")]
        [ProducesResponseType(typeof(GetAddressByHash.Model), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public Task<GetAddressByHash.Model> GetAddressByHash([FromRoute] string hash)
        {
            return Mediator.Send(new GetAddressByHash.Query {Hash = hash});
        }
    }
}
