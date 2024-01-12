using Items.Models.DataTransferObjects.Item;
using Items.Models;
using Items.Queries;
using Microsoft.Extensions.Caching.Memory;
using Items.Models.DataTransferObjects;

namespace Items.Queries.Cache
{
    internal sealed class GetItemsPageQueryCached : CachedQueryBase<PaginatedResult<ItemDto>>
    {
        public GetItemsPageQueryCached(
            int page,
            int pageSize,
            FilterDto? filter,
            string? sort,
            IMemoryCache memoryCache,
            IQuery<PaginatedResult<ItemDto>> query)
            : base(() => GetCacheKey(page, pageSize, filter, sort), memoryCache, query)
        {
            AbsoluteExpiration = TimeSpan.FromMinutes(1);
            SlidingExpiration = TimeSpan.FromMinutes(1);
        }

        private static string GetCacheKey(int page, int pageSize, FilterDto? filter, string? sort)
        {
            return $"ItemsPage:{page}:{pageSize}:{filter}:{sort}";
        }
    }
}
