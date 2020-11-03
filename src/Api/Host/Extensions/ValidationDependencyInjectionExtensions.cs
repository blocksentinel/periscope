using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Periscope.Api.Application.Behaviors;

namespace Periscope.Api.Host.Extensions
{
    public static class ValidationDependencyInjectionExtensions
    {
        public static IMvcBuilder AddValidation(this IMvcBuilder builder)
        {
            return builder.ConfigureApiBehaviorOptions(opt => { opt.SuppressModelStateInvalidFilter = true; })
                .AddFluentValidation(configuration =>
                    configuration.RegisterValidatorsFromAssemblyContaining(typeof(RequestValidationBehavior<,>)));
        }
    }
}
