using Items.Abstractions.Commands;
using Items.Abstractions.Commands.Handlers;
using Items.Data;
using Items.Models;
using Items.Models.Commands;
using Items.Models.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Items.Commands.Handlers
{
    internal sealed class UpdateItemCommandHandler : IUpdateItemCommandHandler
    {
        private readonly DbContextProvider _dbContextProvider;

        public UpdateItemCommandHandler(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task ExecuteAsync(
            UpdateItemCommand updateItemCommand,
            CancellationToken cancellationToken)
        {
            var newItemState = updateItemCommand.ItemDto;
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);

            var item = await dbContext
                .Items
                .Include(i => i.Categories)
                .Where(i => i.Id == updateItemCommand.ItemId)
                .AsSingleQuery()
                .SingleOrDefaultAsync(cancellationToken);

            if (item == default)
            {
                throw new BusinessException(
                    ListOfBusinessErrors.ProductNotFound,
                    new() { { "Id", updateItemCommand.ItemId.ToString() } });
            }

            item.Id = newItemState.Id;
            item.Price = newItemState.Price;
            item.Description = newItemState.Description;
            item.DisplayName = newItemState.DisplayName;
            item.AvailableQuantity = newItemState.AvailableQuantity;
            item.ImageUrl = newItemState.ImageUrl;
            item.OverallRating = newItemState.OverallRating;
            item.Categories = await GetOrCreateItemCategories(dbContext, newItemState.Categories);

            dbContext.Items.Update(item);

            await dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<IList<ItemCategory>> GetOrCreateItemCategories(
            ItemsDbContext dbContext,
            IEnumerable<string> categories)
        {
            var foundCategories = await dbContext
                .ItemsCategory
                .Where(ic => categories.Contains(ic.DisplayName))
                .ToListAsync();

            var notFoundCategories = categories
                .Except(foundCategories.Select(fc => fc.DisplayName))
                .Select(c => new ItemCategory { DisplayName = c });

            foundCategories.AddRange(notFoundCategories);

            return foundCategories;
        }
    }
}
