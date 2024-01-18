using Items.Abstractions.Queries.Common;
using Items.Models.DataTransferObjects.Item;
using Items.Models.Queries;

namespace Items.Abstractions.Queries.Handlers
{
    public interface IItemListQueryHandler
        : IQueryHandler<ItemListQuery, IEnumerable<ItemDto>>
    {

    }
}
