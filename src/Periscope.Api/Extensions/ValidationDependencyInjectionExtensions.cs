using FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Periscope.Core.Behaviors;

namespace Periscope.Api.Extensions
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
