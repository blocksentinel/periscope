using System.Threading.Tasks;
using Cinder.Api.Application.Features.Address;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cinder.Api.Host.Controllers
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
