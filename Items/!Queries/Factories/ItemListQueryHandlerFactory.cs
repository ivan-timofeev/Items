using Items.Queries.Handlers;
using Microsoft.EntityFrameworkCore;
using Items.Data;
using Items.Queries.Cache;
using Items.Abstractions.Queries.Factories;
using Items.Abstractions.Queries.Handlers;
using Items.Abstractions.Services;

namespace Items.Queries.Factories
{
    internal sealed class ItemListQueryHandlerFactory : IItemListQueryHandlerFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly ICacheService _cacheService;

        public ItemListQueryHandlerFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            ICacheService cacheService)
        {
            _dbContextFactory = dbContextFactory;
            _cacheService = cacheService;
        }

        public IItemListQueryHandler CreateHandler()
        {
            var queryHandler = new ItemsListQueryHandler(
                (cancellationToken) => _dbContextFactory.CreateDbContextAsync(cancellationToken));

            return queryHandler;
        }

        public IItemListQueryHandler CreateCachedHandler()
        {
            var queryHandler = CreateHandler();
            var cachedQueryHandler = new ItemListQueryHandlerCacheDecorator(_cacheService, queryHandler);

            return cachedQueryHandler;
        }
    }
}
