using Microsoft.Extensions.DependencyInjection;

namespace Test.Shop.Core
{
    public static class Extensions
    {
        public static IServiceCollection AddCoreLayer(this IServiceCollection services)
        {
            return services;
        }
    }
}
