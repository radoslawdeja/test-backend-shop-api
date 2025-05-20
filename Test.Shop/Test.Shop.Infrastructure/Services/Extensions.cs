using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Test.Shop.Infrastructure.Services.Cron;
using Test.Shop.Infrastructure.Services.Cron.Abstractions;
using Test.Shop.Infrastructure.Services.Cron.Models;
using Test.Shop.Infrastructure.Shared.RabbitMq.Models;

namespace Test.Shop.Infrastructure.Services
{
    public static class Extensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitSettings = configuration.GetSection("RabbitSettings").Get<RabbitSettings>();
            //var cronSettings = configuration.GetSection("CronSettings").Get<CronSettings>();

            if (rabbitSettings is not null)
            {
                services
                    .AddSingleton(rabbitSettings)
                    .AddSingleton(serviceProvider =>
                    {
                        return new ConnectionFactory
                        {
                            Port = rabbitSettings.Port,
                            VirtualHost = rabbitSettings.VirtualHost,
                            UserName = rabbitSettings.Username,
                            Password = rabbitSettings.Password,
                            DispatchConsumersAsync = true
                        };
                    });
            }

            //services
            //.AddExpressionCronJob<IAggregatorService>(x => x.RunAggregators(), cronSettings.ShopSyncCron)

            return services;
        }

        public static IServiceCollection AddCronJob<T>(this IServiceCollection services, Action<IScheduleConfig<T>> options)
            where T : CronJobService
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), "Please provide schedule configurations.");
            }

            var config = new ScheduleConfig<T>();
            options.Invoke(config);

            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(options), "Provider cron expression is invalid.");
            }

            services.AddSingleton<IScheduleConfig<T>>(config);
            services.AddHostedService<T>();

            return services;
        }

        public static IServiceCollection AddExpressionCronJob<TService>(this IServiceCollection services, Func<TService, Task> action, string cronExpression)
            where TService : notnull
        {
            return services.AddExpressionCronJob(action, x => { x.CronExpression = cronExpression; x.TimeZoneInfo = TimeZoneInfo.Local; });
        }

        public static IServiceCollection AddExpressionCronJob<TService>(
            this IServiceCollection services,
            Func<TService, Task> action,
            Action<IScheduleConfig<ExpressionCronJob<TService>>> options) where TService : notnull
        {
            if (options is null)
            {
                throw new ArgumentNullException(nameof(options), "Please provide schedule configurations.");
            }

            var config = new ScheduleConfig<ExpressionCronJob<TService>>();
            options.Invoke(config);

            if (string.IsNullOrWhiteSpace(config.CronExpression))
            {
                throw new ArgumentNullException(nameof(options), "Provider cron expression is invalid.");
            }

            services.AddHostedService(x => new ExpressionCronJob<TService>(action, config, x));

            return services;
        }
    }
}
