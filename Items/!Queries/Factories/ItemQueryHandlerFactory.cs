using Items.Queries.Handlers;
using Microsoft.EntityFrameworkCore;
using Items.Data;
using Items.Queries.Cache;
using Items.Abstractions.Queries.Factories;
using Items.Abstractions.Queries.Handlers;
using Items.Abstractions.Services;

namespace Items.Queries.Factories
{
    internal sealed class ItemQueryHandlerFactory : IItemQueryHandlerFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly ICacheService _cacheService;

        public ItemQueryHandlerFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            ICacheService cacheService)
        {
            _dbContextFactory = dbContextFactory;
            _cacheService = cacheService;
        }

        public IItemQueryHandler CreateHandler()
        {
            var queryHandler = new ItemQueryHandler(
                (cancellationToken) => _dbContextFactory.CreateDbContextAsync(cancellationToken));

            return queryHandler;
        }

        public IItemQueryHandler CreateCachedHandler()
        {
            var queryHandler = CreateHandler();
            var cachedQueryHandler = new ItemQueryHandlerCacheDecorator(_cacheService, queryHandler);

            return cachedQueryHandler;
        }
    }
}
