using Test.Shop.Infrastructure.Services.Cron.Abstractions;

namespace Test.Shop.Infrastructure.Services.Cron
{
    public class ScheduleConfig<T> : IScheduleConfig<T>
    {
        public string CronExpression { get; set; } = string.Empty;

        public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.Local;
    }
}
