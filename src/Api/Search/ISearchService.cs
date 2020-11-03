using System.Threading.Tasks;

namespace Periscope.Api.Search
{
    public interface ISearchService
    {
        Task<SearchResult> ExecuteSearch(string query);
    }
}
