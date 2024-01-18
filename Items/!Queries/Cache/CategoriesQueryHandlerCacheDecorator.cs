using Items.Abstractions.Queries.Handlers;
using Items.Models.Queries;
using Items.Abstractions.Queries.Common;
using Items.Abstractions.Services;
using Items.Models.DataTransferObjects;

namespace Items.Queries.Cache
{
    internal sealed class CategoriesQueryHandlerCacheDecorator
        : CacheDecoratorBase<
            CategoriesQuery,
            ICacheKeyProvider<CategoriesQuery>,
            IEnumerable<CategoryDto>>
        , ICategoriesQueryHandler
    {
        public CategoriesQueryHandlerCacheDecorator(
            ICacheService cacheService,
            ICategoriesQueryHandler queryHandler)
            : base(cacheService, new CacheProvider(), queryHandler)
        {
            AbsoluteExpiration = TimeSpan.FromHours(1);
        }

        private class CacheProvider : ICacheKeyProvider<CategoriesQuery>
        {
            public string GetCacheKey(CategoriesQuery query)
            {
                return "Categories:All";
            }
        }
    }
}
