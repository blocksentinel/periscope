using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;
using Nethereum.Util;
using Periscope.Core.Paging;
using Periscope.Core.SharedKernel;
using Periscope.Data.Repositories;
using Periscope.Documents;

namespace Periscope.Api.Application.Features.Transaction
{
    public class GetTransactionsByAddressHash
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.AddressHash).NotEmpty().Length(42);
                RuleFor(m => m.Page).GreaterThanOrEqualTo(1);
                RuleFor(m => m.Size).InclusiveBetween(1, 100);
            }
        }

        public class Query : IRequest<IPage<Model>>
        {
            public string AddressHash { get; set; }
            public int? Page { get; set; }
            public int? Size { get; set; }
            public SortOrder Sort { get; set; }
        }

        public class Model
        {
            public string BlockHash { get; set; }
            public string BlockNumber { get; set; }
            public string Hash { get; set; }
            public string AddressFrom { get; set; }
            public ulong Timestamp { get; set; }
            public ulong TransactionIndex { get; set; }
            public decimal Value { get; set; }
            public string AddressTo { get; set; }
            public string Gas { get; set; }
            public string GasPrice { get; set; }
            public string Input { get; set; }
            public string Nonce { get; set; }
            public bool Failed { get; set; }
            public string ReceiptHash { get; set; }
            public string GasUsed { get; set; }
            public string CumulativeGasUsed { get; set; }
            public string Error { get; set; }
        }

        public class Handler : IRequestHandler<Query, IPage<Model>>
        {
            private readonly IAddressTransactionRepository _addressTransactionRepository;
            private readonly ISettings _settings;
            private readonly ITransactionRepository _transactionRepository;

            public Handler(IAddressTransactionRepository addressTransactionRepository,
                ITransactionRepository transactionRepository, IOptions<Settings> options)
            {
                _addressTransactionRepository = addressTransactionRepository;
                _transactionRepository = transactionRepository;
                _settings = options.Value;
            }

            public async Task<IPage<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                IPage<string> transactionHashes = await _addressTransactionRepository.GetPagedTransactionHashesByAddressHash(
                    request.AddressHash, request.Page, request.Size, request.Sort, _settings.Performance.QueryCountLimiter,
                    cancellationToken);

                IEnumerable<CinderTransaction> transactions =
                    await _transactionRepository.GetTransactionsByHashes(transactionHashes.Items, cancellationToken);

                IEnumerable<Model> models = transactions.Select(transaction => new Model
                {
                    BlockHash = transaction.BlockHash,
                    BlockNumber = transaction.BlockNumber,
                    Hash = transaction.Hash,
                    AddressFrom = transaction.AddressFrom,
                    Timestamp = ulong.Parse(transaction.TimeStamp),
                    TransactionIndex = ulong.Parse(transaction.TransactionIndex),
                    Value = UnitConversion.Convert.FromWei(BigInteger.Parse(transaction.Value)),
                    AddressTo = transaction.AddressTo,
                    Gas = transaction.Gas,
                    GasPrice = transaction.GasPrice,
                    Input = transaction.Input,
                    Nonce = transaction.Nonce,
                    Failed = transaction.Failed,
                    ReceiptHash = transaction.ReceiptHash,
                    GasUsed = transaction.GasUsed,
                    CumulativeGasUsed = transaction.CumulativeGasUsed,
                    Error = transaction.Error
                });

                return new PagedEnumerable<Model>(models, transactionHashes.Total, transactionHashes.Page,
                    transactionHashes.Size);
            }
        }
    }
}
