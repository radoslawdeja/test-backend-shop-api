using Microsoft.Extensions.DependencyInjection;
using Test.Shop.Application.Services;
using Test.Shop.Application.Services.Interfaces;
using Test.Shop.Application.Validators;
using Test.Shop.Application.Validators.Interfaces;

namespace Test.Shop.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplicationLayer(this IServiceCollection services)
        {
            services
                .AddScoped<IShopDetailsService, ShopDetailsService>()
                .AddScoped<IShopDetailsAddValidator, ShopDetailsAddValidator>()
                .AddScoped<IShopDetailsUpdateValidator, ShopDetailsUpdateValidator>();

            return services;
        }
    }
}
