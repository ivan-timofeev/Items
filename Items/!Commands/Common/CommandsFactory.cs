using Items._Commands;
using Items.Data;
using Items.Models.DataTransferObjects.Item;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Items.Commands
{
    public class CommandsFactory : ICommandsFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly IMemoryCache _memoryCache;

        public CommandsFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            IMemoryCache memoryCache)
        {
            _dbContextFactory = dbContextFactory;
            _memoryCache = memoryCache;
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
                _memoryCache);
        }
    }
}
