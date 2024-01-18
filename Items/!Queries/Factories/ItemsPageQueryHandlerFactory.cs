using Items.Queries.Handlers;
using Microsoft.EntityFrameworkCore;
using Items.Data;
using Items.Queries.Cache;
using Items.Abstractions.Queries.Factories;
using Items.Abstractions.Queries.Handlers;
using Items.Abstractions.Services;

namespace Items.Queries.Factories
{
    internal sealed class ItemsPageQueryHandlerFactory : IItemsPageQueryHandlerFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly ICacheService _cacheService;

        public ItemsPageQueryHandlerFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            ICacheService cacheService)
        {
            _dbContextFactory = dbContextFactory;
            _cacheService = cacheService;
        }

        public IItemsPageQueryHandler CreateHandler()
        {
            var queryHandler = new ItemsPageQueryHandler(
                (cancellationToken) => _dbContextFactory.CreateDbContextAsync(cancellationToken));

            return queryHandler;
        }

        public IItemsPageQueryHandler CreateCachedHandler()
        {
            var queryHandler = CreateHandler();
            var cachedQueryHandler = new ItemsPageQueryHandlerCacheDecorator(_cacheService, queryHandler);

            return cachedQueryHandler;
        }
    }
}
