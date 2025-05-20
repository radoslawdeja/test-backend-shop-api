using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Shop.Infrastructure.Diagnostics
{
    public static class Extensions
    {
        public static IServiceCollection AddDiagnosticsMiddleware(this IServiceCollection services)
            => services.AddSingleton<DiagnosticsMiddleware>();

        public static IApplicationBuilder UseDiagnosticsMiddleware(this IApplicationBuilder builder)
            => builder.UseMiddleware<DiagnosticsMiddleware>();
    }
}
