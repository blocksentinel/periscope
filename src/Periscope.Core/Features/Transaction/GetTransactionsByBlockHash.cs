using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Nethereum.Util;
using Periscope.Core.Data.Repositories;
using Periscope.Core.Documents;
using Periscope.Core.Extensions;

namespace Periscope.Core.Features.Transaction
{
    public class GetTransactionsByBlockHash
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.BlockHash).NotEmpty().Length(66);
            }
        }

        public class Query : IRequest<IEnumerable<Model>>
        {
            public string BlockHash { get; set; }
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

        public class Handler : IRequestHandler<Query, IEnumerable<Model>>
        {
            private readonly ITransactionRepository _transactionRepository;

            public Handler(ITransactionRepository transactionRepository)
            {
                _transactionRepository = transactionRepository;
            }

            public async Task<IEnumerable<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                IEnumerable<CinderTransaction> transactions = await _transactionRepository
                    .GetTransactionsByBlockHash(request.BlockHash, cancellationToken)
                    .AnyContext();

                return transactions.Select(transaction => new Model
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
            }
        }
    }
}
