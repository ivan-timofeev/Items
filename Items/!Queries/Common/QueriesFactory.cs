using Items.Data;
using Items.Models;
using Items.Models.DataTransferObjects;
using Items.Models.DataTransferObjects.Item;
using Items.Queries.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Items.Queries
{
    internal sealed class QueriesFactory : IQueriesFactory
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;
        private readonly IMemoryCache _memoryCache;

        public QueriesFactory(
            IDbContextFactory<ItemsDbContext> dbContextFactory,
            IMemoryCache memoryCache)
        {
            _dbContextFactory = dbContextFactory;
            _memoryCache = memoryCache;
        }

        public IQuery<PaginatedResult<ItemDto>> CreateGetItemsPageQuery(
            int page,
            int pageSize,
            FilterDto? filter,
            string? sort,
            bool useMemoryCache = true)
        {
            var query = new GetItemsPageQuery(page, pageSize, filter, sort, _dbContextFactory.CreateDbContext());

            return useMemoryCache
                ? new GetItemsPageQueryCached(page, pageSize, filter, sort, _memoryCache, query)
                : query;
        }

        public IQuery<IEnumerable<ItemDto>> CreateGetItemsListQuery(IReadOnlyCollection<Guid> itemsIds, bool useMemoryCache = true)
        {
            var originQuery = new GetItemsListQuery(itemsIds, _dbContextFactory.CreateDbContext());

            return useMemoryCache
                ? new GetItemsListQueryCached(itemsIds, _memoryCache, originQuery)
                : originQuery;
        }

        public IQuery<ItemDto> CreateGetItemQuery(Guid itemId, bool useMemoryCache = true)
        {
            var originQuery = new GetItemQuery(itemId, _dbContextFactory.CreateDbContext());

            return useMemoryCache
                ? new GetItemQueryCached(itemId, _memoryCache, originQuery)
                : originQuery;
        }

        public IQuery<IEnumerable<CategoryDto>> CreateGetAllCategoriesQuery(bool useMemoryCache = true)
        {
            var originQuery = new GetAllCategoriesQuery(_dbContextFactory.CreateDbContext());

            return useMemoryCache
                ? new GetAllCategoriesQueryCached(_memoryCache, originQuery)
                : originQuery;
        }
    }
}
