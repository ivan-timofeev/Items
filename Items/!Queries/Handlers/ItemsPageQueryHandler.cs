using Items.Data;
using Items.Models.DataTransferObjects.Item;
using Items.Models;
using Microsoft.EntityFrameworkCore;
using Items.Abstractions.Queries.Handlers;
using Items.Models.Queries;

namespace Items.Queries.Handlers
{
    internal sealed class ItemsPageQueryHandler : IItemsPageQueryHandler
    {
        private readonly DbContextProvider _dbContextProvider;

        public ItemsPageQueryHandler(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<PaginatedResult<ItemDto>> ExecuteAsync(
            ItemsPageQuery itemsPageQuery,
            CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);

            var itemsQuery = dbContext
                .Items
                .Include(i => i.Categories)
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
                    });

            if (itemsPageQuery.Filter != default)
            {
                if (itemsPageQuery.Filter.SelectedCategories.Any())
                    itemsQuery = itemsQuery.Where(i => i.Categories.Any(c => itemsPageQuery.Filter.SelectedCategories.Contains(c)));

                if (itemsPageQuery.Filter.SelectedPriceRange.From != null)
                    itemsQuery = itemsQuery.Where(i => i.Price >= itemsPageQuery.Filter.SelectedPriceRange.From);

                if (itemsPageQuery.Filter.SelectedPriceRange.To != null)
                    itemsQuery = itemsQuery.Where(i => i.Price <= itemsPageQuery.Filter.SelectedPriceRange.To);
            }

            if (itemsPageQuery.Sort == "price-desc")
            {
                itemsQuery = itemsQuery.OrderByDescending(i => i.Price);
            }
            else if (itemsPageQuery.Sort == "price-asc")
            {
                itemsQuery = itemsQuery.OrderBy(i => i.Price);
            }

            var items = await itemsQuery
                .Skip((itemsPageQuery.Page - 1) * itemsPageQuery.PageSize)
                .Take(itemsPageQuery.PageSize)
                .ToArrayAsync(cancellationToken);

            var count = itemsQuery.Count();

            return new PaginatedResult<ItemDto>()
            {
                TotalElementsCount = count,
                PageElementsCount = items.Length,
                Elements = items,
                CurrentPageNumber = itemsPageQuery.Page,
                MaxPageNumber = (int)Math.Ceiling((decimal)count / itemsPageQuery.PageSize)
            };
        }
    }
}
