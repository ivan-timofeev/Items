using Items.Models.DataTransferObjects.Item;
using Items.Queries;
using Microsoft.Extensions.Caching.Memory;

namespace Items.Queries.Cache
{
    internal sealed class GetItemsListQueryCached : CachedQueryBase<IEnumerable<ItemDto>>
    {
        public GetItemsListQueryCached(
            IReadOnlyCollection<Guid> itemsIds,
            IMemoryCache memoryCache,
            IQuery<IEnumerable<ItemDto>> query)
            : base(itemsIds.GetCacheKey, memoryCache, query)
        {

        }
    }

    public static class Helper
    {
        public static string GetCacheKey<T>(this IEnumerable<T> collection)
        {
            var dynamicCacheKeyPart = string.Join(',', collection.Select(x => x.ToString()));
            return $"List:{typeof(T).Name}:{dynamicCacheKeyPart}";
        }
    }
}
