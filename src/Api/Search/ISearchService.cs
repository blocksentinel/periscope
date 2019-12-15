using System.Threading.Tasks;

namespace Cinder.Api.Search
{
    public interface ISearchService
    {
        Task<SearchResult> ExecuteSearch(string query);
    }
}
