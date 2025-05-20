using Test.Shop.Core.Shared.Time;

namespace Test.Shop.Infrastructure.Shared.Time
{
    internal sealed class DateTimeClock : IClock
    {
        public DateTime Current() => DateTime.UtcNow;
    }
}
