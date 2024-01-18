using Items.Models.DataTransferObjects.Item;
using Items.Abstractions.Queries.Handlers;
using Items.Models.Queries;
using Items.Abstractions.Queries.Common;
using Items.Abstractions.Services;

namespace Items.Queries.Cache
{
    internal sealed class ItemListQueryHandlerCacheDecorator
        : CacheDecoratorBase<
            ItemListQuery,
            ICacheKeyProvider<ItemListQuery>,
            IEnumerable<ItemDto>>
        , IItemListQueryHandler
    {
        public ItemListQueryHandlerCacheDecorator(
            ICacheService cacheService,
            IItemListQueryHandler queryHandler)
            : base(cacheService, new CacheProvider(), queryHandler)
        {
            AbsoluteExpiration = TimeSpan.FromHours(1);
        }

        private class CacheProvider : ICacheKeyProvider<ItemListQuery>
        {
            public string GetCacheKey(ItemListQuery query)
            {
                // ItemList:{guid},{guid},{guid} ...
                return $"ItemList:{string.Join(",", query.ItemsIds.Select(i => i.ToString()))}";
            }
        }
    }
}
