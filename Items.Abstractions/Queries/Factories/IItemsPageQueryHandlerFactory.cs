using Items.Abstractions.Queries.Handlers;
using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Items.Models.Queries;

namespace Items.Abstractions.Queries.Factories
{
    public interface IItemsPageQueryHandlerFactory
        : IQueryHandlerFactory<IItemsPageQueryHandler, ItemsPageQuery, PaginatedResult<ItemDto>>
    {

    }
}
