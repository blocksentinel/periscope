using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Periscope.Core;
using Periscope.Core.Behaviors;

namespace Periscope.Api.Extensions
{
    public static class MediatorDependencyInjectionExtensions
    {
        public static void AddMediator(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPreProcessorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestPerformanceBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));

            services.AddMediatR(typeof(VersionInfo));
        }
    }
}
