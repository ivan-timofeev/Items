using Items.Data;
using Items.Helpers;
using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Items.Commands
{
    public class UpdateItemCommand : ICommand
    {
        private readonly Guid _itemId;
        private readonly ItemDto _itemDto;

        private readonly ItemsDbContext _dbContext;
        private readonly IMemoryCache _memoryCache;

        public UpdateItemCommand(
            Guid itemId,
            ItemDto itemDto,
            ItemsDbContext dbContext,
            IMemoryCache memoryCache)
        {
            _itemId = itemId;
            _itemDto = itemDto;

            _dbContext = dbContext;
            _memoryCache = memoryCache;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            var item = await _dbContext
                .Items
                .Include(i => i.Categories)
                .Where(i => i.Id == _itemId)
                .SingleOrThrowNotFoundExceptionAsync(nameof(Item), _itemId.ToString(), cancellationToken);

            item.Id = _itemDto.Id;
            item.Price = _itemDto.Price;
            item.Description = _itemDto.Description;
            item.DisplayName = _itemDto.DisplayName;
            item.AvailableQuantity = _itemDto.AvailableQuantity;
            item.ImageUrl = _itemDto.ImageUrl;
            item.OverallRating = _itemDto.OverallRating;
            item.Categories = await GetOrCreateItemCategories(_itemDto.Categories);

            _dbContext.Update(item);
            _memoryCache.Remove(Item.GetCacheKey(_itemId));
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        private async Task<IList<ItemCategory>> GetOrCreateItemCategories(IEnumerable<string> categories)
        {
            var foundCategories = await _dbContext
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
