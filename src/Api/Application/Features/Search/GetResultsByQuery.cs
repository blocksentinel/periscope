using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Periscope.Api.Search;
using Periscope.Core.Extensions;

namespace Periscope.Api.Application.Features.Search
{
    public class GetResultsByQuery
    {
        public class Validator : AbstractValidator<Query>
        {
            public Validator()
            {
                RuleFor(m => m.Term).NotEmpty();
            }
        }

        public class Query : IRequest<Model>
        {
            public string Term { get; set; }
        }

        public class Model
        {
            public string Id { get; set; }
            public SearchResultType Type { get; set; }
        }

        public class Handler : IRequestHandler<Query, Model>
        {
            private readonly ISearchService _searchService;

            public Handler(ISearchService searchService)
            {
                _searchService = searchService;
            }

            public async Task<Model> Handle(Query request, CancellationToken cancellationToken)
            {
                SearchResult result = await _searchService.ExecuteSearch(request.Term).AnyContext();

                return new Model {Id = result.Id, Type = result.Type};
            }
        }
    }
}
