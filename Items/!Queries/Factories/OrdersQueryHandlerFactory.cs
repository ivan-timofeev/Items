using Microsoft.EntityFrameworkCore;
using Items.Data;
using Items.Abstractions.Queries.Factories;
using Items.Abstractions.Queries.Handlers;
using Items.Abstractions.Services;
using Items.Queries.Handlers;

namespace Items.Queries.Factories
{
    internal sealed class OrdersQueryHandlerFactory : IOrdersQueryHanlderFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly ICacheService _cacheService;

        public OrdersQueryHandlerFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            ICacheService cacheService)
        {
            _dbContextFactory = dbContextFactory;
            _cacheService = cacheService;
        }

        public IOrdersQueryHandler CreateHandler()
        {
            var queryHandler = new OrdersQueryHandler(
                (cancellationToken) => _dbContextFactory.CreateDbContextAsync(cancellationToken));

            return queryHandler;
        }

        public IOrdersQueryHandler CreateCachedHandler()
        {
            throw new NotImplementedException();
        }
    }
}
