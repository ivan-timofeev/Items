using Items.Abstractions.Queries.Handlers;
using Items.Models;
using Items.Models.DataTransferObjects.Order;
using Items.Models.Queries;

namespace Items.Abstractions.Queries.Factories
{
    public interface IOrdersQueryHanlderFactory
        : IQueryHandlerFactory<IOrdersQueryHandler, OrdersQueryBase, PaginatedResult<OrderDto>>
    {

    }
}
