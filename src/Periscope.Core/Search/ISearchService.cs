using System.Threading.Tasks;

namespace Periscope.Core.Search
{
    public interface ISearchService
    {
        Task<SearchResult> ExecuteSearch(string query);
    }
}
