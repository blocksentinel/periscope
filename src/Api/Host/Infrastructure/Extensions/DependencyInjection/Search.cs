using Cinder.Api.Search;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Cinder.Extensions.DependencyInjection
{
    public static class Search
    {
        public static void AddSearch(this IServiceCollection services)
        {
            services.AddSingleton<ISearchService, SearchService>();
        }
    }
}
