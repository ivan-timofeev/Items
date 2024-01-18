using Items.Abstractions.Services;
using Items.Data;
using Items.Models.DataTransferObjects.Item;
using Microsoft.EntityFrameworkCore;

namespace Items.Commands
{
    public class CommandsFactory : ICommandsFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly ICacheService _cacheService;

        public CommandsFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            ICacheService cacheService)
        {
            _dbContextFactory = dbContextFactory;
            _cacheService = cacheService;
        }

        public ICommand CreateEnsureIsDatabaseAliveCommand()
        {
            return new EnsureIsDatabaseAliveCommand(_dbContextFactory.CreateDbContext());
        }

        public ICommand CreateUpdateItemCommand(Guid itemId, ItemDto itemDto)
        {
            return new UpdateItemCommand(
                itemId,
                itemDto,
                _dbContextFactory.CreateDbContext(),
                _cacheService);
        }
    }
}
