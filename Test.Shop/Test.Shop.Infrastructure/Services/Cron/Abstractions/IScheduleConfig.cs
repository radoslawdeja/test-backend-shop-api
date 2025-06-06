﻿namespace Test.Shop.Infrastructure.Services.Cron.Abstractions
{
    public interface IScheduleConfig<T>
    {
        string CronExpression { get; set; }

        TimeZoneInfo TimeZoneInfo { get; set; }
    }
}
