using System;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Documents;
using FluentValidation;
using MediatR;
using Nethereum.Hex.HexTypes;
using Nethereum.Parity;
using Nethereum.Util;

namespace Cinder.Api.Infrastructure.Features.Address
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
            public decimal Balance { get; set; }
            public ulong? BlocksMined { get; set; }
            public ulong? TransactionCount { get; set; }
            public ulong? Timestamp { get; set; }
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly IAddressRepository _addressRepository;
            private readonly IWeb3Parity _web3;

            public Handler(IAddressRepository addressRepository, IWeb3Parity web3)
            {
                _addressRepository = addressRepository;
                _web3 = web3;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                CinderAddress address = await _addressRepository.GetAddressByHash(request.Hash, cancellationToken)
                    .ConfigureAwait(false);

                if (address != null)
                {
                    return new Model
                    {
                        Hash = address.Hash,
                        Balance = address.Balance,
                        BlocksMined = address.BlocksMined,
                        TransactionCount = address.TransactionCount,
                        Timestamp = address.Timestamp
                    };
                }

                string hash = request.Hash.ToLowerInvariant();
                HexBigInteger balance = await _web3.Eth.GetBalance.SendRequestAsync(hash).ConfigureAwait(false);
                address = new CinderAddress
                {
                    Hash = hash,
                    Balance = UnitConversion.Convert.FromWei(balance),
                    Timestamp = (ulong) DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    ForceRefresh = true
                };
                await _addressRepository.UpsertAddress(address, cancellationToken).ConfigureAwait(false);

                return new Model
                {
                    Hash = address.Hash,
                    Balance = address.Balance,
                    BlocksMined = address.BlocksMined,
                    TransactionCount = address.TransactionCount,
                    Timestamp = address.Timestamp
                };
            }
        }
    }
}
