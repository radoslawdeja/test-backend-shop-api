using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Test.Shop.Core.Shared.Time;
using Test.Shop.Infrastructure.DAL;
using Test.Shop.Infrastructure.Shared.Time;

namespace Test.Shop.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddSingleton<IClock, DateTimeClock>()
                .AddDatabase(configuration);

            return services;
        }
    }
}
