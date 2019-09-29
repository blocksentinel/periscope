using System;
using System.Net;
using System.Threading.Tasks;
using Cinder.Api.Infrastructure.Dtos;
using Cinder.Api.Infrastructure.HttpResponses;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Cinder.Api.Infrastructure.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e)
            {
                await HandleExceptionAsync(context, e);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            string error = "An unexpected error occured.";
            string logError = "Unexpected exception caught";
            switch (exception)
            {
                //case ResourceException _:
                //    statusCode = HttpStatusCode.Forbidden;
                //    error = "There was an issue accessing the requested object.";
                //    logError = "ResourceException exception caught";
                //    break;
                case InvalidOperationException _:
                    statusCode = HttpStatusCode.BadRequest;
                    logError = "Invalid operation exception caught";
                    break;
            }

            _logger.LogError(exception, logError);

            ErrorHttpResponse errorResponse = new ErrorHttpResponse(error);
            if (_env.IsDevelopment())
            {
                errorResponse.Exception = exception;
            }

            ErrorHttpResponseDto body = errorResponse.ToDto();
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int) statusCode;

            return context.Response.WriteAsync(body.ToString());
        }
    }
}
