using Items.Data;
using Items.Models.DataTransferObjects;
using Microsoft.EntityFrameworkCore;

namespace Items.Queries
{
    internal sealed class GetAllCategoriesQuery : IQuery<IEnumerable<CategoryDto>>
    {
        private readonly ItemsDbContext _dbContext;

        public GetAllCategoriesQuery(ItemsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<CategoryDto>> ExecuteAsync(CancellationToken cancellationToken)
        {
            return await _dbContext
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
