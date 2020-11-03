using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Periscope.Api.Application;
using Periscope.Api.Application.Behaviors;

namespace Periscope.Api.Host.Extensions
{
    public static class MediatorDependencyInjectionExtensions
    {
        public static void AddMediator(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            services.AddMediatR(typeof(ApplicationModule));
        }
    }
}
