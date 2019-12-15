using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Cinder.Api.Application.Behaviors
{
    public class RequestPerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<TRequest> _logger;
        private readonly Stopwatch _timer;

        public RequestPerformanceBehavior(ILogger<TRequest> logger, IHttpContextAccessor httpContextAccessor)
        {
            _timer = new Stopwatch();
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            _timer.Start();

            TResponse response = await next();

            _timer.Stop();

            if (_timer.ElapsedMilliseconds <= 500)
            {
                return response;
            }

            string name = typeof(TRequest).FullName;

            _logger.LogWarning(
                "Long Running Request: {Name} ({ElapsedMilliseconds} milliseconds) | Payload: {@Request} | User: {User}", name,
                _timer.ElapsedMilliseconds, request, _httpContextAccessor?.HttpContext?.User?.Identity?.Name);

            return response;
        }
    }
}
