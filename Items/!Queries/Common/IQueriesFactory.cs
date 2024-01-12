using Items.Models.DataTransferObjects;
using Items.Models;
using Items.Models.DataTransferObjects.Item;

namespace Items.Queries
{
    public interface IQueriesFactory
    {
        IQuery<PaginatedResult<ItemDto>> CreateGetItemsPageQuery(
            int page,
            int pageSize,
            FilterDto? filter,
            string? sort,
            bool useMemoryCache = true);

        IQuery<IEnumerable<ItemDto>> CreateGetItemsListQuery(
            IReadOnlyCollection<Guid> itemsIds,
            bool useMemoryCache = true);

        IQuery<ItemDto> CreateGetItemQuery(
            Guid itemId,
            bool useMemoryCache = true);
    }
}
