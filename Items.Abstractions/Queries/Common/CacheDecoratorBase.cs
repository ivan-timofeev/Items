using Items.Abstractions.Services;
using Items.Models;

namespace Items.Abstractions.Queries.Common
{
    public abstract class CacheDecoratorBase<TQuery, TCacheKeyProvider, TResponse>
        : IQueryHandler<TQuery, TResponse>
        where TQuery : class
        where TCacheKeyProvider : class, ICacheKeyProvider<TQuery>
        where TResponse : class
    {
        private readonly ICacheService _cacheService;
        private readonly ICacheKeyProvider<TQuery> _cacheKeyProvider;
        private readonly IQueryHandler<TQuery, TResponse> _queryHandler;

        protected TimeSpan SlidingExpiration { get; set; }
        protected TimeSpan AbsoluteExpiration { get; set; }

        public CacheDecoratorBase(
            ICacheService cacheService,
            ICacheKeyProvider<TQuery> cacheKeyProvider,
            IQueryHandler<TQuery, TResponse> queryHandler)
        {
            _cacheService = cacheService;
            _cacheKeyProvider = cacheKeyProvider;
            _queryHandler = queryHandler;

            SlidingExpiration = TimeSpan.FromMinutes(1);
            AbsoluteExpiration = TimeSpan.FromMinutes(1);
        }

        public async Task<TResponse> ExecuteAsync(TQuery query, CancellationToken cancellationToken)
        {
            var cacheKey = _cacheKeyProvider.GetCacheKey(query);

            _cacheService.TryGetValue<TResponse>(cacheKey, out var item);

            if (item != null)
                return item;

            item = await _queryHandler.ExecuteAsync(query, cancellationToken);

            var cacheEntryOptions = new CacheEntryOptions
            {
                SlidingExpiration = SlidingExpiration,
                AbsoluteExpiration = AbsoluteExpiration
            };

            _cacheService.Set(cacheKey, item, cacheEntryOptions);

            return item;
        }
    }
}
