namespace Items.Models
{
    public class CacheEntry
    {
        public required CacheEntryOptions CacheEntryOptions { get; init; }
        public required object Value { get; init; }
        public required DateTime EnterDateTimeUtc { get; init; }
        public required DateTime ExpirationDateTimeUtc { get; set; }
        public DateTime AbsoluteExpirationDateTimeUtc => EnterDateTimeUtc.Add(CacheEntryOptions.AbsoluteExpiration);
    }
}
