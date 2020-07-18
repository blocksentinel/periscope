using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cinder.Core.Paging;
using Cinder.Documents;
using Cinder.Extensions;
using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.Hex.HexTypes;
using Block = Nethereum.RPC.Eth.DTOs.Block;

namespace Cinder.Data.Repositories
{
    public class BlockRepository : RepositoryBase<CinderBlock>, IBlockRepository
    {
        public BlockRepository(IMongoClient client, string databaseName) : base(client, databaseName, CollectionName.Blocks) { }

        public Task UpsertBlockAsync(Block source)
        {
            CinderBlock document = source.MapToStorageEntityForUpsert<CinderBlock>();
            document.Sha3Uncles = source.Sha3Uncles;
            document.Uncles = source.Uncles;
            document.UncleCount = source.Uncles.Length;
            return UpsertDocumentAsync(document);
        }

        public async Task<IBlockView> FindByBlockNumberAsync(HexBigInteger blockNumber)
        {
            FilterDefinition<CinderBlock> filter =
                CreateDocumentFilter(new CinderBlock {BlockNumber = blockNumber.Value.ToString()});
            CinderBlock response = await Collection.Find(filter).SingleOrDefaultAsync().AnyContext();

            return response;
        }

        public async Task<IPage<CinderBlock>> GetBlocks(int? page = null, int? size = null, SortOrder sort = SortOrder.Ascending,
            CancellationToken cancellationToken = default)
        {
            long total = await Collection.EstimatedDocumentCountAsync(cancellationToken: cancellationToken).AnyContext();
            IFindFluent<CinderBlock, CinderBlock> query = Collection.Find(FilterDefinition<CinderBlock>.Empty)
                .Skip(((page ?? 1) - 1) * (size ?? 10))
                .Limit(size ?? 10);

            switch (sort)
            {
                case SortOrder.Ascending:
                    query = query.SortBy(block => block.Id);
                    break;
                case SortOrder.Descending:
                    query = query.SortByDescending(block => block.Id);
                    break;
            }

            List<CinderBlock> blocks = await query.ToListAsync(cancellationToken).AnyContext();

            return new PagedEnumerable<CinderBlock>(blocks, (int) total, page ?? 1, size ?? 10);
        }

        public Task<CinderBlock> GetBlockByHash(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();

            return Collection.Find(Builders<CinderBlock>.Filter.Eq(document => document.Hash, hash))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<string> GetBlockHashIfExists(string hash, CancellationToken cancellationToken = default)
        {
            hash = hash.ToLowerInvariant();
            var result = await Collection.Find(Builders<CinderBlock>.Filter.Eq(document => document.Hash, hash))
                .Project(block => new {block.Hash})
                .SingleOrDefaultAsync(cancellationToken)
                .AnyContext();

            return result?.Hash;
        }

        public Task<CinderBlock> GetBlockByNumber(ulong number, CancellationToken cancellationToken = default)
        {
            return Collection.Find(Builders<CinderBlock>.Filter.Eq(document => document.BlockNumber, number.ToString()))
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<string> GetBlockNumberIfExists(ulong number, CancellationToken cancellationToken = default)
        {
            var result = await Collection
                .Find(Builders<CinderBlock>.Filter.Eq(document => document.BlockNumber, number.ToString()))
                .Project(block => new {block.BlockNumber})
                .SingleOrDefaultAsync(cancellationToken)
                .AnyContext();

            return result?.BlockNumber;
        }

        public async Task<ulong> GetBlocksMinedCountByAddressHash(string addressHash,
            CancellationToken cancellationToken = default)
        {
            addressHash = addressHash.ToLowerInvariant();
            long total = await Collection.Find(Builders<CinderBlock>.Filter.Eq(document => document.Miner, addressHash))
                .CountDocumentsAsync(cancellationToken)
                .AnyContext();

            return (ulong) total;
        }
    }
}
