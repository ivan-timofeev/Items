using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using Items.Models;
using Items.Data;
using Items.Models.Exceptions;
using Items.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Items.Queries
{

    internal sealed class GetItemQuery : IQuery<ItemDto>
    {
        private readonly Guid _itemId;
        private readonly ItemsDbContext _dbContext;

        public GetItemQuery(Guid itemId, ItemsDbContext dbContext)
        {
            _itemId = itemId;
            _dbContext = dbContext;
        }

        public async Task<ItemDto> ExecuteAsync(CancellationToken cancellationToken)
        {
            return await _dbContext
                .Items
                .Where(i => i.Id == _itemId)
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
                    _itemId.ToString(),
                    cancellationToken);
        }
    }
}
