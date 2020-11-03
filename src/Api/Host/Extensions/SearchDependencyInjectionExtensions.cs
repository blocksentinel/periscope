using Microsoft.Extensions.DependencyInjection;
using Periscope.Api.Search;

namespace Periscope.Api.Host.Extensions
{
    public static class SearchDependencyInjectionExtensions
    {
        public static void AddSearch(this IServiceCollection services)
        {
            services.AddSingleton<ISearchService, SearchService>();
        }
    }
}
