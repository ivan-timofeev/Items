using Items.Models;
using Items.Abstractions.Queries.Common;
using Items.Models.Queries;
using Items.Models.DataTransferObjects.Order;

namespace Items.Abstractions.Queries.Handlers
{
    public interface IOrdersQueryHandler
        : IQueryHandler<OrdersQueryBase, PaginatedResult<OrderDto>>
    {

    }
}
