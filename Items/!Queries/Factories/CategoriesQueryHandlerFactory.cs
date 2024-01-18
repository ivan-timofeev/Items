using Items.Abstractions.Queries.Factories;
using Items.Abstractions.Queries.Handlers;
using Items.Abstractions.Services;
using Items.Data;
using Items.Queries.Cache;
using Items.Queries.Handlers;
using Microsoft.EntityFrameworkCore;

namespace Items.Queries.Factories
{
    internal sealed class CategoriesQueryHandlerFactory
        : ICategoriesQueryHandlerFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly ICacheService _cacheService;

        public CategoriesQueryHandlerFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            ICacheService cacheService)
        {
            _dbContextFactory = dbContextFactory;
            _cacheService = cacheService;
        }

        public ICategoriesQueryHandler CreateHandler()
        {
            var queryHandler = new CategoriesQueryHandler(
                (cancellationToken) => _dbContextFactory.CreateDbContextAsync(cancellationToken));

            return queryHandler;
        }

        public ICategoriesQueryHandler CreateCachedHandler()
        {
            var queryHandler = CreateHandler();
            var cachedQueryHandler = new CategoriesQueryHandlerCacheDecorator(_cacheService, queryHandler);

            return cachedQueryHandler;
        }
    }
}
