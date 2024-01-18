using Items.Models.DataTransferObjects.Item;
using Items.Data;
using Microsoft.EntityFrameworkCore;
using Items.Abstractions.Queries.Handlers;
using Items.Models.Queries;

namespace Items.Queries.Handlers
{
    internal sealed class ItemsListQueryHandler : IItemListQueryHandler
    {
        private readonly DbContextProvider _dbContextProvider;

        public ItemsListQueryHandler(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<IEnumerable<ItemDto>> ExecuteAsync(
            ItemListQuery getItemsListQuery,
            CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);

            return await dbContext
                .Items
                .Include(i => i.Categories)
                .Where(i => getItemsListQuery.ItemsIds.Contains(i.Id))
                .Select(i =>
                    new ItemDto
                    {
                        Id = i.Id,
                        AvailableQuantity = i.AvailableQuantity,
                        Description = i.Description,
                        DisplayName = i.DisplayName,
                        ImageUrl = i.ImageUrl,
                        OverallRating = i.OverallRating,
                        Price = i.Price,
                        Categories = i.Categories
                            .Select(c => c.DisplayName)
                            .ToArray()
                    })
                .ToArrayAsync(cancellationToken);
        }
    }
}
