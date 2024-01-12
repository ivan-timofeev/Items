using Items.Models.DataTransferObjects.Item;
using Items.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Items.Queries.Cache
{
    internal sealed class GetItemQueryCached : CachedQueryBase<ItemDto>
    {
        public GetItemQueryCached(
            Guid itemId,
            IMemoryCache memoryCache,
            IQuery<ItemDto> query)
            : base(() => Item.GetCacheKey(itemId), memoryCache, query)
        {
            SlidingExpiration = TimeSpan.FromHours(1);
            AbsoluteExpiration = TimeSpan.FromDays(1);
        }
    }
}
