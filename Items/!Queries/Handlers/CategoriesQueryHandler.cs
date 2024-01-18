using Items.Abstractions.Queries.Handlers;
using Items.Data;
using Items.Models.DataTransferObjects;
using Items.Models.Queries;
using Microsoft.EntityFrameworkCore;

namespace Items.Queries.Handlers
{
    internal sealed class CategoriesQueryHandler : ICategoriesQueryHandler
    {
        private readonly DbContextProvider _dbContextProvider;

        public CategoriesQueryHandler(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<IEnumerable<CategoryDto>> ExecuteAsync(
            CategoriesQuery categoriesQuery,
            CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);

            return await dbContext
                .ItemsCategory
                .Select(ic =>
                    new CategoryDto
                    {
                        DisplayName = ic.DisplayName,
                        ProductsInCategory = ic.Items.Count
                    })
                .OrderByDescending(ic => ic.ProductsInCategory)
                .ThenByDescending(ic => ic.DisplayName)
                .ToArrayAsync(cancellationToken);
        }
    }
}
