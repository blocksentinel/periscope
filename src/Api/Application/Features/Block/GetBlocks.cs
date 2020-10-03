using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Data.Repositories;
using Cinder.Documents;
using Cinder.Extensions;
using FluentValidation;
using MediatR;

namespace Cinder.Api.Application.Features.Block
{
    public class GetBlocks
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Page).GreaterThanOrEqualTo(1);
                RuleFor(m => m.Size).InclusiveBetween(1, 100);
            }
        }

        public class Query : IRequest<IPage<Model>>
        {
            public int? Page { get; set; }
            public int? Size { get; set; }
            public SortOrder Sort { get; set; }
        }

        public class Model
        {
            public string BlockNumber { get; set; }
            public string Hash { get; set; }
            public string ParentHash { get; set; }
            public string Nonce { get; set; }
            public string ExtraData { get; set; }
            public string Difficulty { get; set; }
            public string TotalDifficulty { get; set; }
            public ulong Size { get; set; }
            public string Miner { get; set; }
            public string MinerDisplay { get; set; }
            public ulong GasLimit { get; set; }
            public ulong GasUsed { get; set; }
            public ulong Timestamp { get; set; }
            public ulong TransactionCount { get; set; }
            public string[] Uncles { get; set; }
            public ulong UncleCount { get; set; }
            public string Sha3Uncles { get; set; }
            public decimal Value { get; set; }
            public decimal Fees { get; set; }
        }

        public class Handler : IRequestHandler<Query, IPage<Model>>
        {
            private readonly IAddressMetaRepository _addressMetaRepository;
            private readonly IBlockRepository _blockRepository;

            public Handler(IBlockRepository blockRepository, IAddressMetaRepository addressMetaRepository)
            {
                _blockRepository = blockRepository;
                _addressMetaRepository = addressMetaRepository;
            }

            public async Task<IPage<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                IPage<CinderBlock> page = await _blockRepository
                    .GetBlocks(request.Page, request.Size, request.Sort, cancellationToken)
                    .AnyContext();
                IEnumerable<CinderAddressMeta> metas = await _addressMetaRepository
                    .GetByAddresses(page.Items.Select(block => block.Miner).Distinct(), cancellationToken)
                    .AnyContext();
                IEnumerable<Model> models = page.Items.Select(block => new Model
                {
                    BlockNumber = block.BlockNumber,
                    Difficulty = block.Difficulty,
                    ExtraData = block.ExtraData,
                    GasLimit = ulong.Parse(block.GasLimit),
                    GasUsed = ulong.Parse(block.GasUsed),
                    Hash = block.Hash,
                    Miner = block.Miner,
                    MinerDisplay = metas.FirstOrDefault(miner => miner.Hash == block.Miner)?.Name,
                    Nonce = block.Nonce,
                    ParentHash = block.ParentHash,
                    Size = ulong.Parse(block.Size),
                    Timestamp = ulong.Parse(block.Timestamp),
                    TransactionCount = (ulong) block.TransactionCount,
                    TotalDifficulty = block.TotalDifficulty,
                    Uncles = block.Uncles,
                    UncleCount = (ulong) block.UncleCount,
                    Sha3Uncles = block.Sha3Uncles,
                    Value = block.Value,
                    Fees = block.Fees
                });

                return new PagedEnumerable<Model>(models, page.Total, page.Page, page.Size);
            }
        }
    }
}
