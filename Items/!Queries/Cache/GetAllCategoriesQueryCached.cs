using Items.Models.DataTransferObjects;
using Microsoft.Extensions.Caching.Memory;

namespace Items.Queries.Cache
{
    internal sealed class GetAllCategoriesQueryCached : CachedQueryBase<IEnumerable<CategoryDto>>
    {
        public GetAllCategoriesQueryCached(
            IMemoryCache memoryCache,
            IQuery<IEnumerable<CategoryDto>> query)
            : base(() => "AllCategories", memoryCache, query)
        {
            
        }
    }
}
