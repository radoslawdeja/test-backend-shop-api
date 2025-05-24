using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
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
                .InitAutoMapper()
                .AddSingleton<IClock, DateTimeClock>()
                .AddDatabase(configuration);

            return services;
        }

        public static IServiceCollection InitAutoMapper(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var configuration = new MapperConfiguration(x =>
            {
                x.AllowNullCollections = true;
                x.AddMaps(assembly);
                x.CreateMap<DateOnly?, DateTime?>().ConvertUsing(src => src.HasValue ? ((DateOnly)src).ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified) : null);
                x.CreateMap<DateTime?, DateOnly?>().ConvertUsing(src => src.HasValue ? DateOnly.FromDateTime(src.Value) : null);
                x.CreateMap<DateOnly, DateTime>().ConvertUsing(src => src.ToDateTime(TimeOnly.MinValue, DateTimeKind.Unspecified));
                x.CreateMap<DateTime, DateOnly>().ConvertUsing(src => DateOnly.FromDateTime(src));
            });

            services.AddSingleton(x => configuration.CreateMapper());

            return services;
        }
    }
}
