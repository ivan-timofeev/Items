using Items.Abstractions.Queries.Handlers;
using Items.Data;
using Items.Helpers;
using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Items.Models.Queries;

namespace Items.Queries.Handlers
{
    internal sealed class ItemQueryHandler : IItemQueryHandler
    {
        private readonly DbContextProvider _dbContextProvider;

        public ItemQueryHandler(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<ItemDto> ExecuteAsync(ItemQuery query, CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);

            return await dbContext
                .Items
                .Where(i => i.Id == query.ItemId)
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
                .SingleOrThrowNotFoundExceptionAsync(
                    nameof(Item),
                    query.ItemId.ToString(),
                    cancellationToken);
        }
    }
}
