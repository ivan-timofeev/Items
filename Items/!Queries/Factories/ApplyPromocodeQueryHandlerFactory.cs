using Microsoft.EntityFrameworkCore;
using Items.Data;
using Items.Abstractions.Queries.Factories;
using Items.Abstractions.Queries.Handlers;
using Items.Queries.Handlers;

namespace Items.Queries.Factories
{
    internal sealed class ApplyPromocodeQueryHandlerFactory : IApplyPromocodeQueryHandlerFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;

        public ApplyPromocodeQueryHandlerFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public IApplyPromocodeQueryHandler CreateHandler()
        {
            var queryHandler = new ApplyPromocodeQueryHandler(
                (cancellationToken) => _dbContextFactory.CreateDbContextAsync(cancellationToken));

            return queryHandler;
        }

        public IApplyPromocodeQueryHandler CreateCachedHandler()
        {
            throw new NotImplementedException();
        }
    }
}
