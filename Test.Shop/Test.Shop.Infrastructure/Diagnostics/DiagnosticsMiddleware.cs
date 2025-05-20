using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Test.Shop.Infrastructure.Diagnostics
{
    internal sealed class DiagnosticsMiddleware : IMiddleware
    {
        private readonly ILogger<DiagnosticsMiddleware> _logger;

        public DiagnosticsMiddleware(ILogger<DiagnosticsMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var stopwatch = new Stopwatch();

            stopwatch.Start();
            await next(context);
            stopwatch.Stop();

            _logger.LogInformation("Completed request method: {Method}. Path: {Path}. Duration: {Duration} ms",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds);
        }
    }
}
