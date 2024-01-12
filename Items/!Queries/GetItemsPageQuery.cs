using Items.Data;
using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects;
using Items.Models;
using Microsoft.EntityFrameworkCore;

namespace Items.Queries
{
    internal sealed class GetItemsPageQuery : IQuery<PaginatedResult<ItemDto>>
    {
        private readonly int _page;
        private readonly int _pageSize;
        private readonly FilterDto? _filter;
        private readonly string? _sort;
        private readonly ItemsDbContext _dbContext;

        public GetItemsPageQuery(
            int page,
            int pageSize,
            FilterDto? filter,
            string? sort,
            ItemsDbContext dbContext)
        {
            _page = page.EnsureArgumentCorrect(p => p >= 1,
                "Page must be greater than or equal to 1.");

            _pageSize = pageSize.EnsureArgumentCorrect(ps => ps >= 1 && ps <= 20,
                "Page size must be in range [1, 20].");

            _filter = filter.EnsureArgumentCorrect(f =>
                f == null
                    || f.SelectedPriceRange.From == null
                    || f.SelectedPriceRange.To == null
                    || f?.SelectedPriceRange?.To >= f?.SelectedPriceRange?.From,
                "Price to must be bigger than price from.");

            _sort = sort;
            _dbContext = dbContext;
        }

        public async Task<PaginatedResult<ItemDto>> ExecuteAsync(CancellationToken cancellationToken)
        {
            var itemsQuery = _dbContext
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

            if (_filter != default)
            {
                if (_filter.SelectedCategories.Any())
                    itemsQuery = itemsQuery.Where(i => i.Categories.Any(c => _filter.SelectedCategories.Contains(c)));

                if (_filter.SelectedPriceRange.From != null)
                    itemsQuery = itemsQuery.Where(i => i.Price >= _filter.SelectedPriceRange.From);

                if (_filter.SelectedPriceRange.To != null)
                    itemsQuery = itemsQuery.Where(i => i.Price <= _filter.SelectedPriceRange.To);
            }

            if (_sort == "price-desc")
            {
                itemsQuery = itemsQuery.OrderByDescending(i => i.Price);
            }
            else if (_sort == "price-asc")
            {
                itemsQuery = itemsQuery.OrderBy(i => i.Price);
            }

            var items = await itemsQuery
                .Skip((_page - 1) * _pageSize)
                .Take(_pageSize)
                .ToArrayAsync(cancellationToken);

            var count = itemsQuery.Count();

            return new PaginatedResult<ItemDto>()
            {
                TotalElementsCount = count,
                PageElementsCount = items.Length,
                Elements = items,
                CurrentPageNumber = _page,
                MaxPageNumber = (int)Math.Ceiling((decimal)count / _pageSize)
            };
        }
    }

    public static class Test
    {
        public static T EnsureArgumentCorrect<T>(this T argument, Func<T, bool> predicate, string exceptionMessage)
        {
            if (!predicate(argument))
                throw new ArgumentException(exceptionMessage);
            return argument;
        }
    }
}
