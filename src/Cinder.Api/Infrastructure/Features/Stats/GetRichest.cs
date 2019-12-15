using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Data.Repositories;
using Cinder.Documents;
using FluentValidation;
using MediatR;

namespace Cinder.Api.Infrastructure.Features.Stats
{
    public class GetRichest
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Page).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1000);
                RuleFor(m => m.Size).GreaterThanOrEqualTo(1).LessThanOrEqualTo(100);
            }
        }

        public class Query : IRequest<IPage<Model>>
        {
            public int? Page { get; set; }
            public int? Size { get; set; }
        }

        public class Model
        {
            public int Rank { get; set; }
            public string Hash { get; set; }
            public decimal Balance { get; set; }
        }

        public class Handler : IRequestHandler<Query, IPage<Model>>
        {
            private readonly IAddressRepository _addressRepository;

            public Handler(IAddressRepository addressRepository)
            {
                _addressRepository = addressRepository;
            }

            public async Task<IPage<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                IPage<CinderAddress> page = await _addressRepository.GetRichest(request.Page, request.Size, cancellationToken)
                    .ConfigureAwait(false);

                int rank = 1 * (page.Page - 1) * page.Size + 1;
                IEnumerable<Model> models = page.Items.Select(address => new Model
                {
                    Rank = rank++, Hash = address.Hash, Balance = address.Balance
                });

                return new PagedEnumerable<Model>(models, page.Total, page.Page, page.Size);
            }
        }
    }
}
