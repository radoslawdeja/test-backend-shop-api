using Microsoft.Extensions.DependencyInjection;

namespace Test.Shop.Infrastructure.Exceptions
{
    public static class Extensions
    {
        public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
        {
            services.AddExceptionHandler<GlobalExceptionHandler>();

            return services;
        }
    }
}
