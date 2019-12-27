using System.Threading;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Extensions;
using MediatR;

namespace Cinder.Api.Application.Features.Stats
{
    public class GetSupply
    {
        public class Query : IRequest<Model> { }

        public class Model
        {
            public decimal Supply { get; set; }
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly IAddressRepository _addressRepository;

            public Handler(IAddressRepository addressRepository)
            {
                _addressRepository = addressRepository;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                decimal supply = await _addressRepository.GetSupply(cancellationToken).AnyContext();

                return new Model {Supply = supply};
            }
        }
    }
}
