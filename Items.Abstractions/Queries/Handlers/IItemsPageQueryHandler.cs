using Items.Models.DataTransferObjects.Item;
using Items.Models;
using Items.Abstractions.Queries.Common;
using Items.Models.Queries;

namespace Items.Abstractions.Queries.Handlers
{
    public interface IItemsPageQueryHandler
        : IQueryHandler<ItemsPageQuery, PaginatedResult<ItemDto>>
    {

    }
}
