using Items.Abstractions.Commands.Handlers;
using Items.Data;
using Items.Models.Commands;
using Microsoft.EntityFrameworkCore;

namespace Items.Commands.Handlers
{
    internal sealed class CheckDatabaseCommandHandler : ICheckDatabaseCommandHandler
    {
        private readonly DbContextProvider _dbContextProvider;

        public CheckDatabaseCommandHandler(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task ExecuteAsync(CheckDatabaseCommand command, CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);
            await dbContext.Items.FirstOrDefaultAsync(cancellationToken);
        }
    }
}
