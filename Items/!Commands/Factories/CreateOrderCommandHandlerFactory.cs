using Items.Abstractions.Commands.Factories;
using Items.Abstractions.Commands.Handlers;
using Items.Abstractions.Services;
using Items.Commands.Handlers;
using Items.Data;
using Microsoft.EntityFrameworkCore;

namespace Items.Commands.Factories
{
    internal sealed class CreateOrderCommandHandlerFactory : ICreateOrderCommandHandlerFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateOrderCommandHandlerFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            IDateTimeProvider dateTimeProvider)
        {
            _dbContextFactory = dbContextFactory;
            _dateTimeProvider = dateTimeProvider;
        }

        public ICreateOrderCommandHandler CreateHandler()
        {
            return new CreateOrderCommandHandler(
                (cancalletionToken) => _dbContextFactory.CreateDbContextAsync(cancalletionToken),
                _dateTimeProvider);
        }
    }
}
