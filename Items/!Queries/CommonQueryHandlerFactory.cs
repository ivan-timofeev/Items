using Microsoft.EntityFrameworkCore;
using Items.Data;
using Items.Abstractions.Queries;

namespace Items.Queries
{
    internal sealed class CommonQueryHandlerFactory<THandlerInterface, THandlerImplementation>
        : IQueryHandlerFactory<THandlerInterface>
    {
        private readonly DbContextProvider _dbContextProvider;
        private readonly IServiceProvider _serviceProvider;

        public CommonQueryHandlerFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            IServiceProvider serviceProvider)
        {
            _dbContextProvider = (cancalletionToken) => dbContextFactory.CreateDbContextAsync(cancalletionToken);
            _serviceProvider = serviceProvider;
        }

        public THandlerInterface CreateHandler()
        {
            var constructorArguments = new object[] { _dbContextProvider };

            var createdHandler = ActivatorUtilities.CreateInstance(_serviceProvider, typeof(THandlerImplementation), constructorArguments)
                ?? throw new InvalidOperationException("Something went wrong.");

            return (THandlerInterface)createdHandler;
        }
    }
}
