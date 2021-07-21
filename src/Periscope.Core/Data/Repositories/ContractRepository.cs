using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nethereum.BlockchainProcessing.BlockStorage.Entities;
using Nethereum.BlockchainProcessing.BlockStorage.Entities.Mapping;
using Nethereum.RPC.Eth.DTOs;
using Periscope.Core.Documents;
using Periscope.Core.Extensions;

namespace Periscope.Core.Data.Repositories
{
    public class ContractRepository : RepositoryBase<CinderContract>, IContractRepository
    {
        private readonly ConcurrentDictionary<string, CinderContract> _cachedContracts = new();

        public ContractRepository(IMongoClient client, string databaseName) :
            base(client, databaseName, CollectionName.Contracts) { }

        public async Task FillCache()
        {
            using IAsyncCursor<CinderContract> cursor =
                await Collection.FindAsync(FilterDefinition<CinderContract>.Empty).AnyContext();
            while (await cursor.MoveNextAsync())
            {
                IEnumerable<CinderContract> batch = cursor.Current;
                foreach (CinderContract contract in batch)
                {
                    _cachedContracts.AddOrUpdate(contract.Address, contract, (s, existingContract) => contract);
                }
            }
        }

        public async Task UpsertAsync(ContractCreationVO contractCreation)
        {
            CinderContract contract = contractCreation.MapToStorageEntityForUpsert<CinderContract>();
            await UpsertDocumentAsync(contract).AnyContext();

            _cachedContracts.AddOrUpdate(contract.Address, contract, (s, existingContract) => contract);
        }

        public async Task<bool> ExistsAsync(string contractAddress)
        {
            contractAddress = contractAddress.ToLowerInvariant();
            IContractView existing = await FindByAddressAsync(contractAddress).AnyContext();

            return existing != null;
        }

        public async Task<IContractView> FindByAddressAsync(string contractAddress)
        {
            FilterDefinition<CinderContract> filter = CreateDocumentFilter(contractAddress);
            CinderContract response = await Collection.Find(filter).SingleOrDefaultAsync().AnyContext();

            return response;
        }

        public bool IsCached(string contractAddress)
        {
            return _cachedContracts.ContainsKey(contractAddress);
        }
    }
}
