using Items.Models.DataTransferObjects.Item;
using Items.Abstractions.Queries.Common;
using Items.Models.Queries;

namespace Items.Abstractions.Queries.Handlers
{
    public interface IItemQueryHandler
        : IQueryHandler<ItemQuery, ItemDto>
    {

    }
}
