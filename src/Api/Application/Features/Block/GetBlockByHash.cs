using System.Threading;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Documents;
using Cinder.Extensions;
using FluentValidation;
using MediatR;

namespace Cinder.Api.Application.Features.Block
{
    public class GetBlockByHash
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Hash).NotEmpty().Length(66);
            }
        }

        public class Query : IRequest<Model>
        {
            public string Hash { get; set; }
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
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly IBlockRepository _blockRepository;
            private readonly IMinerRepository _minerRepository;

            public Handler(IBlockRepository blockRepository, IMinerRepository minerRepository)
            {
                _blockRepository = blockRepository;
                _minerRepository = minerRepository;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                CinderBlock block = await _blockRepository.GetBlockByHash(request.Hash, cancellationToken).AnyContext();

                if (block == null)
                {
                    return null;
                }

                CinderMiner miner = await _minerRepository.GetByAddressOrDefault(block.Miner, cancellationToken).AnyContext();

                return new Model
                {
                    BlockNumber = block.BlockNumber,
                    Difficulty = block.Difficulty,
                    ExtraData = block.ExtraData,
                    GasLimit = ulong.Parse(block.GasLimit),
                    GasUsed = ulong.Parse(block.GasUsed),
                    Hash = block.Hash,
                    Miner = block.Miner,
                    MinerDisplay = miner?.Name,
                    Nonce = block.Nonce,
                    ParentHash = block.ParentHash,
                    Size = ulong.Parse(block.Size),
                    Timestamp = ulong.Parse(block.Timestamp),
                    TransactionCount = (ulong) block.TransactionCount,
                    TotalDifficulty = block.TotalDifficulty,
                    Uncles = block.Uncles,
                    UncleCount = (ulong) block.UncleCount,
                    Sha3Uncles = block.Sha3Uncles
                };
            }
        }
    }
}
