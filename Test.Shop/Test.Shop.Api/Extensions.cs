using Asp.Versioning;
using Microsoft.FeatureManagement;
using Microsoft.FeatureManagement.FeatureFilters;
using Microsoft.OpenApi.Models;
using Serilog;
using Test.Shop.Api.AppStart.Swagger;
using Test.Shop.Api.Endpoints.ShopEndpoints;

namespace Test.Shop.Api
{
    internal static class Extensions
    {
        public static IServiceCollection AddFeature(this IServiceCollection services)
        {
            services
                .AddFeatureManagement()
                .AddFeatureFilter<PercentageFilter>();

            return services;
        }

        public static IHostBuilder AddSerilog(this IHostBuilder host)
        {
            host.UseSerilog((context, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

            return host;
        }

        public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            var (title, version, _) = GetSwaggerOptions(configuration);

            services
                .AddEndpointsApiExplorer()
                .AddSwaggerGen(x =>
                {
                    x.SwaggerDoc(version, new OpenApiInfo { Title = title, Version = version });
                });

            return services;
        }

        public static IApplicationBuilder UseSwaggerExtension(this IApplicationBuilder app, IConfiguration configuration)
        {
            var (title, version, routePrefix) = GetSwaggerOptions(configuration);

            app.UseSwagger(x =>
            {
                x.RouteTemplate = $"{routePrefix}/docs/{{documentName}}/swagger.json";
            });

            app.UseSwaggerUI(x =>
            {
                x.SwaggerEndpoint($"/{routePrefix}/docs/{version}/swagger.json", $"{title} {version}");
                x.RoutePrefix = $"{routePrefix}/swagger";
            });

            return app;
        }

        private static (string, string, string) GetSwaggerOptions(IConfiguration configuration)
        {
            var options = configuration.GetSection("SwaggerOptions").Get<SwaggerOptions>() ?? new SwaggerOptions();

            return (options.Title, options.Version, options.RoutePrefix);
        }

        public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder builder)
        {
            var apiVersion = builder
                .NewApiVersionSet()
                .HasApiVersion(new ApiVersion(1))
                .ReportApiVersions()
                .Build();

            var group = builder
                .MapGroup("v{varsion:apiVersion}")
                .WithApiVersionSet(apiVersion);

            // Map endpoints
            group.MapShopEndpoints();

            // Add health check
            builder.MapHealthChecks("health");

            return builder;
        }
    }
}
