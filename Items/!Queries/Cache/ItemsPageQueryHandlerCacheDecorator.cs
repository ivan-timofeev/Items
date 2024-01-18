using Items.Models.DataTransferObjects.Item;
using Items.Abstractions.Queries.Handlers;
using Items.Models.Queries;
using Items.Abstractions.Queries.Common;
using Items.Abstractions.Services;
using Items.Models;

namespace Items.Queries.Cache
{
    internal sealed class ItemsPageQueryHandlerCacheDecorator
        : CacheDecoratorBase<
            ItemsPageQuery,
            ICacheKeyProvider<ItemsPageQuery>,
            PaginatedResult<ItemDto>>
        , IItemsPageQueryHandler
    {
        public ItemsPageQueryHandlerCacheDecorator(
            ICacheService cacheService,
            IItemsPageQueryHandler queryHandler)
            : base(cacheService, new CacheProvider(), queryHandler)
        {
            AbsoluteExpiration = TimeSpan.FromHours(1);
        }

        private class CacheProvider : ICacheKeyProvider<ItemsPageQuery>
        {
            public string GetCacheKey(ItemsPageQuery query)
            {
                return $"ItemsPage:{query.Page}:{query.PageSize}:{query.Filter}:{query.Sort}";
            }
        }
    }
}
