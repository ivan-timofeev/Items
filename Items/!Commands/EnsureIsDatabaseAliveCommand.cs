using Items.Commands;
using Items.Data;
using Microsoft.EntityFrameworkCore;

namespace Items.Commands
{
    internal sealed class EnsureIsDatabaseAliveCommand : ICommand
    {
        private readonly ItemsDbContext _dbContext;

        public EnsureIsDatabaseAliveCommand(ItemsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _dbContext.Items.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
