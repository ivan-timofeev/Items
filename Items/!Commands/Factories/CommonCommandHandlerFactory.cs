using Items.Abstractions.Commands.Factories;
using Items.Abstractions.Services;
using Items.Data;
using Microsoft.EntityFrameworkCore;
using Quartz;
using System.Reflection.Emit;

namespace Items.Commands.Factories
{
    internal sealed class CommonCommandHandlerFactory<THandlerInterface, THandlerImplementation>
        : ICommandHandlerFactory<THandlerInterface>
    {
        private readonly DbContextProvider _dbContextProvider;
        private readonly IServiceProvider _serviceProvider;

        public CommonCommandHandlerFactory(
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

            return (THandlerInterface) createdHandler;
        }
    }
}
