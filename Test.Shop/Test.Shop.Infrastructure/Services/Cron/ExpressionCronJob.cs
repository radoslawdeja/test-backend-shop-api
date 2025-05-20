using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Test.Shop.Infrastructure.Services.Cron.Abstractions;

namespace Test.Shop.Infrastructure.Services.Cron
{
    public sealed class ExpressionCronJob<TService>(Func<TService, Task> action, IScheduleConfig<ExpressionCronJob<TService>> config, IServiceProvider serviceProvider)
        : CronJobService(config.CronExpression, config.TimeZoneInfo)
        where TService : notnull
    {
        private readonly ILogger<ExpressionCronJob<TService>> _logger = serviceProvider.GetRequiredService<ILogger<ExpressionCronJob<TService>>>();

        private readonly IServiceProvider _serviceProvider = serviceProvider;

        private readonly Func<TService, Task> _action = action;

        public override async Task DoWork(CancellationToken cancellationToken)
        {
            string serviceName = typeof(TService).Name;
            string methodName = _action.Method.Name;

            _logger.LogInformation("ExpressionCronJob start working on service {ServiceName}, method {MethodName}. Timestamp: {Timestamp}", serviceName, methodName, DateTimeOffset.UtcNow);

            await InvokeAsync();

            _logger.LogInformation("ExpressionCronJob is working on service {ServiceName}, method {MethodName}. Timestamp: {Timestamp}", serviceName, methodName, DateTimeOffset.UtcNow);
        }

        public async Task InvokeAsync()
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                TService service = scope.ServiceProvider.GetRequiredService<TService>();
                await _action(service);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DailyCronJob is invoke action error: {Message},{Timestamp}", ex.Message, DateTimeOffset.UtcNow);
            }
        }
    }
}
