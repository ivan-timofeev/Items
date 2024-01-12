using Microsoft.Extensions.Caching.Memory;

namespace Items.Queries
{
    internal abstract class CachedQueryBase<T> : ICachedQuery<T>
    {
        private readonly Func<string> _cacheKeyProvider;
        private readonly IMemoryCache _memoryCache;
        private readonly IQuery<T> _query;

        protected TimeSpan SlidingExpiration {  get; set; }
        protected TimeSpan AbsoluteExpiration { get; set; }

        public CachedQueryBase(
            Func<string> cacheKeyProvider,
            IMemoryCache memoryCache,
            IQuery<T> query)
        {
            _cacheKeyProvider = cacheKeyProvider;
            _memoryCache = memoryCache;
            _query = query;

            SlidingExpiration = TimeSpan.FromMinutes(1);
            AbsoluteExpiration = TimeSpan.FromHours(1);
        }

        public async Task<T> ExecuteAsync(CancellationToken cancellationToken)
        {
            var cacheKey = _cacheKeyProvider.Invoke();

            _memoryCache.TryGetValue<T>(cacheKey, out var item);

            if (item != null)
                return item;

            item = await _query.ExecuteAsync(cancellationToken);

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(SlidingExpiration)
                .SetAbsoluteExpiration(AbsoluteExpiration);

            _memoryCache.Set(cacheKey, item, cacheEntryOptions);

            return item;
        }
    }
}
