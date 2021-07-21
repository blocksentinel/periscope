using Microsoft.Extensions.DependencyInjection;
using Periscope.Core.Search;

namespace Periscope.Api.Extensions
{
    public static class SearchDependencyInjectionExtensions
    {
        public static void AddSearch(this IServiceCollection services)
        {
            services.AddSingleton<ISearchService, SearchService>();
        }
    }
}
