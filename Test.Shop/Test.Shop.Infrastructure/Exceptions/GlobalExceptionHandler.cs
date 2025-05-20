using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Test.Shop.Infrastructure.Exceptions
{
    internal sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly IProblemDetailsService _pdService;

        public GlobalExceptionHandler(IProblemDetailsService pdService)
        {
            _pdService = pdService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = exception switch
            {
                ApplicationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var problemDetails = new ProblemDetails
            {
                Title = "An error occured",
                Detail = exception.Message,
                Type = exception.GetType().Name
            };

            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

            var problemDetailsContext = new ProblemDetailsContext
            {
                HttpContext = httpContext,
                Exception = exception,
                ProblemDetails = problemDetails
            };

            return await _pdService.TryWriteAsync(problemDetailsContext);
        }
    }
}
