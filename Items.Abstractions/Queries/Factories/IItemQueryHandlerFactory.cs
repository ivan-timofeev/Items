using Items.Abstractions.Queries.Handlers;
using Items.Models.DataTransferObjects.Item;
using Items.Models.Queries;

namespace Items.Abstractions.Queries.Factories
{
    public interface IItemQueryHandlerFactory
        : IQueryHandlerFactory<IItemQueryHandler, ItemQuery, ItemDto>
    {

    }
}
