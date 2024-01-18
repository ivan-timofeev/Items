namespace Items.Abstractions.Services
{
    public interface IDateTimeProvider
    {
        DateTime GetCurrentDateTimeUtc();
    }
}
