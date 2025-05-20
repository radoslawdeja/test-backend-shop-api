using Asp.Versioning;
using Microsoft.AspNetCore.Http.Features;
using Test.Shop.Api;
using Test.Shop.Infrastructure;
using Test.Shop.Core;
using Test.Shop.Application;
using Test.Shop.Api.AppStart.FileProvider;
using Test.Shop.Infrastructure.Diagnostics;
using Serilog;
using Test.Shop.Infrastructure.Exceptions;
using Test.Shop.Infrastructure.DAL;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

const string varName = "DOTNET_RUNNING_IN_CONTAINER";
var runningInContainer = bool.TryParse(Environment.GetEnvironmentVariable(varName), out var isRunningInContainer) && isRunningInContainer;

var configFolder = "config";

if (runningInContainer && !string.IsNullOrWhiteSpace(configFolder) && Directory.Exists(configFolder))
{
    configuration
        .AddJsonFile(ConfigMapFileProvider.FromRelativePath(configFolder), "appsettings.json", optional: false, reloadOnChange: true)
        .AddJsonFile(ConfigMapFileProvider.FromRelativePath(configFolder), $"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
}

builder.Services.AddHealthChecks();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);

        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
    };
});

builder.Services.AddExceptionHandlers();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1);
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader());
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'V";
        options.SubstituteApiVersionInUrl = true;
    });

builder.Services
    .AddHttpContextAccessor()
    .AddSwagger(configuration)
    .AddFeature()
    .AddCoreLayer()
    .AddApplicationLayer()
    .AddInfrastructureLayer(configuration)
    .AddDiagnosticsMiddleware()
    .AddCors();

builder.Host.AddSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(options => options.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
}

app.UseSwaggerExtension(configuration);
app.UseSerilogRequestLogging();
app.UseDiagnosticsMiddleware();
app.UseExceptionHandler();

app.MapEndpoints();
app.InitializeDatabase();

app.UseHttpsRedirection();

await app.RunAsync();