using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cinder.Data.Repositories;
using Cinder.Extensions;

namespace Cinder.Api.Search
{
    public class SearchService : ISearchService
    {
        private readonly IAddressRepository _addressRepository;
        private readonly IBlockRepository _blockRepository;
        private readonly ITransactionRepository _transactionRepository;

        public SearchService(IAddressRepository addressRepository, IBlockRepository blockRepository,
            ITransactionRepository transactionRepository)
        {
            _addressRepository = addressRepository;
            _blockRepository = blockRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<SearchResult> ExecuteSearch(string input)
        {
            string query = Regex.Replace(input ?? "", @"\s+", "");
            SearchResult searchResult = new SearchResult();

            if (Regex.IsMatch(query, "^(0x)?[0-9a-f]{40}$", RegexOptions.IgnoreCase))
            {
                query = query.StartsWith("0x") ? query : $"0x{query}";

                string result = await _addressRepository.GetAddressHashIfExists(query).AnyContext();
                if (string.IsNullOrEmpty(result))
                {
                    return searchResult;
                }

                searchResult.Id = result;
                searchResult.Type = SearchResultType.AddressHash;
            }
            else if (Regex.IsMatch(query, "^(0x)?[0-9a-f]{64}$", RegexOptions.IgnoreCase))
            {
                query = query.StartsWith("0x") ? query : $"0x{query}";

                string result = await _blockRepository.GetBlockHashIfExists(query).AnyContext();
                if (!string.IsNullOrEmpty(result))
                {
                    searchResult.Id = result;
                    searchResult.Type = SearchResultType.BlockHash;

                    return searchResult;
                }

                result = await _transactionRepository.GetTransactionHashIfExists(query).AnyContext();
                if (string.IsNullOrEmpty(result))
                {
                    return searchResult;
                }

                searchResult.Id = result;
                searchResult.Type = SearchResultType.TransactionHash;
            }
            else if (ulong.TryParse(query, out ulong blockNumber))
            {
                string result = await _blockRepository.GetBlockHashByBlockNumberIfExists(blockNumber).AnyContext();
                if (string.IsNullOrEmpty(result))
                {
                    return searchResult;
                }

                searchResult.Id = result;
                searchResult.Type = SearchResultType.BlockHash;
            }

            return searchResult;
        }
    }
}
