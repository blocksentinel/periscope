using System.Linq;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cinder.Api.Host.Controllers
{
    [ApiController]
    [Produces("application/json")]
    public class ErrorController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(ILogger<ErrorController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        [Route("/error")]
        public IActionResult Error()
        {
            IExceptionHandlerFeature context = HttpContext.Features.Get<IExceptionHandlerFeature>();
            IActionResult result;
            string logMessage = "Unexpected exception caught";

            switch (context.Error)
            {
                case ValidationException error:
                    logMessage = null;
                    result = ValidationProblem(new ValidationProblemDetails(
                        error.Errors.ToDictionary(failure => failure.PropertyName, failure => new[] {failure.ErrorMessage})));
                    break;
                default:
                    result = _environment.IsDevelopment()
                        ? Problem(context.Error.StackTrace, title: context.Error.Message)
                        : Problem();
                    break;
            }

            if (!string.IsNullOrEmpty(logMessage))
            {
                _logger.LogError(context.Error, logMessage);
            }

            return result;
        }
    }
}
