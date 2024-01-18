namespace Items.Models
{
    public class CacheEntryOptions
    {
        public required TimeSpan SlidingExpiration { get; set; }
        public required TimeSpan AbsoluteExpiration { get; set; }
    }
}
