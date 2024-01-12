using Items.Models.DataTransferObjects.Item;
using Items.Data;
using Microsoft.EntityFrameworkCore;

namespace Items.Queries
{
    internal sealed class GetItemsListQuery : IQuery<IEnumerable<ItemDto>>
    {
        private readonly IReadOnlyCollection<Guid> _itemsIds;
        private readonly ItemsDbContext _dbContext;

        public GetItemsListQuery(
            IReadOnlyCollection<Guid> itemsIds,
            ItemsDbContext dbContext)
        {
            _itemsIds = itemsIds;
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<ItemDto>> ExecuteAsync(CancellationToken cancellationToken)
        {
            return await _dbContext
                .Items
                .Include(i => i.Categories)
                .Where(i => _itemsIds.Contains(i.Id))
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
