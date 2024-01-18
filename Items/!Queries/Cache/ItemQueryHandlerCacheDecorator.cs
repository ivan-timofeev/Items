using Items.Models.DataTransferObjects.Item;
using Items.Services;
using Items.Abstractions.Queries.Handlers;
using Items.Models.Queries;
using Items.Abstractions.Queries.Common;
using Items.Abstractions.Services;

namespace Items.Queries.Cache
{
    internal sealed class ItemQueryHandlerCacheDecorator
        : CacheDecoratorBase<
            ItemQuery,
            ICacheKeyProvider<ItemQuery>,
            ItemDto>
        , IItemQueryHandler
    {
        public ItemQueryHandlerCacheDecorator(
            ICacheService cacheService,
            IQueryHandler<ItemQuery, ItemDto> queryHandler)
            : base(cacheService, new CacheProvider(), queryHandler)
        {
            AbsoluteExpiration = TimeSpan.FromHours(1);
        }

        private class CacheProvider : ICacheKeyProvider<ItemQuery>
        {
            public string GetCacheKey(ItemQuery query)
            {
                return $"Items:{query.ItemId}";
            }
        }
    }
}
