namespace Periscope.Api.Search
{
    public class SearchResult
    {
        public string Id { get; set; }
        public SearchResultType Type { get; set; } = SearchResultType.Error;
    }

    public enum SearchResultType
    {
        Error,
        AddressHash,
        BlockHash,
        BlockNumber,
        TransactionHash
    }
}
