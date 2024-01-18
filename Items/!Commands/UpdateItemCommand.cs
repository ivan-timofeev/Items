using Items.Abstractions.Services;
using Items.Data;
using Items.Helpers;
using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Items.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Items.Commands
{
    internal sealed class UpdateItemCommand : ICommand
    {
        private readonly Guid _itemId;
        private readonly ItemDto _itemDto;

        private readonly ItemsDbContext _dbContext;
        private readonly ICacheService _cacheService;

        public UpdateItemCommand(
            Guid itemId,
            ItemDto itemDto,
            ItemsDbContext dbContext,
            ICacheService cacheService)
        {
            _itemId = itemId;
            _itemDto = itemDto;

            _dbContext = dbContext;
            _cacheService = cacheService;
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

            _dbContext.Items.Update(item);
            _cacheService.Delete(Item.GetCacheKey(_itemId));
            _cacheService.Delete("ItemsPage:.*");
            _cacheService.Delete("ItemList:.*");
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
