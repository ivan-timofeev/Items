using Items.Abstractions.Services;

namespace Items.Services
{
    internal sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTime GetCurrentDateTimeUtc()
        {
            return DateTime.UtcNow;
        }
    }
}
