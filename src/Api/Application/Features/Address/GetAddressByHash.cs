using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Documents;
using Cinder.Extensions;
using FluentValidation;
using MediatR;

namespace Cinder.Api.Application.Features.Address
{
    public class GetAddressByHash
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Hash).NotEmpty().Length(42);
            }
        }

        public class Query : IRequest<Model>
        {
            public string Hash { get; set; }
        }

        public class Model
        {
            public string Hash { get; set; }
            public string Name { get; set; }
            public string Website { get; set; }
            public decimal Balance { get; set; }
            public ulong? BlocksMined { get; set; }
            public ulong? TransactionCount { get; set; }
            public ulong? Timestamp { get; set; }
            public ICollection<string> Tags { get; set; } = new List<string>();
            public IDictionary<string, decimal> BalanceHistory { get; set; } = new Dictionary<string, decimal>();
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly IAddressMetaRepository _addressMetaRepository;
            private readonly IAddressRepository _addressRepository;

            public Handler(IAddressRepository addressRepository, IAddressMetaRepository addressMetaRepository)
            {
                _addressRepository = addressRepository;
                _addressMetaRepository = addressMetaRepository;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                CinderAddress address = await _addressRepository.GetAddressByHash(request.Hash, cancellationToken).AnyContext();

                if (address == null)
                {
                    return null;
                }

                CinderAddressMeta meta = await _addressMetaRepository.GetByAddressOrDefault(address.Hash, cancellationToken)
                    .AnyContext();

                return new Model
                {
                    Hash = address.Hash,
                    Name = meta?.Name,
                    Website = meta?.Website,
                    Balance = address.Balance,
                    BlocksMined = address.BlocksMined,
                    TransactionCount = address.TransactionCount,
                    Timestamp = address.Timestamp,
                    Tags = meta?.Tags ?? new List<string>(),
                    BalanceHistory = address.BalanceHistory ?? new Dictionary<string, decimal>()
                };
            }
        }
    }
}
