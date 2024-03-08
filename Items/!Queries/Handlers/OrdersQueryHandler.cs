using Items.Abstractions.Queries.Handlers;
using Items.Data;
using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Items.Models.DataTransferObjects.Order;
using Items.Models.Queries;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Items.Queries.Handlers
{
    public class OrdersQueryHandler : IOrdersQueryHandler
    {
        private readonly DbContextProvider _dbContextProvider;

        public OrdersQueryHandler(DbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public async Task<PaginatedResult<OrderDto>> ExecuteAsync(
            OrdersQueryBase ordersQuery,
            CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);

            var orders = dbContext
                .Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .Include(o => o.DeliveryDetails)
                .Include(o => o.OrderStatusHistory)
                .Skip((ordersQuery.Page - 1) * ordersQuery.PageSize)
                .Take(ordersQuery.PageSize);

            orders = FilterByQueryType(orders, ordersQuery);

            var result = await orders
                .Select(o =>
                    new OrderDto
                    {
                        OrderItems = o.OrderItems.Select(oi => new OrderDto.OrderItemDto
                        {
                            Quantity = oi.Quantity,
                            DisplayName = oi.Item.DisplayName,
                            ItemId = oi.Item.Id,
                            ImageUrl = oi.Item.ImageUrl,
                            Price = oi.Price
                        }),
                        DeliveryDetails = new DeliveryDetailsDto
                        {
                            Email = o.DeliveryDetails.Email,
                            FirstName = o.DeliveryDetails.FirstName,
                            LastName = o.DeliveryDetails.LastName,
                            CompanyName = o.DeliveryDetails.CompanyName
                        },
                        OrderStatusHistory = o.OrderStatusHistory
                            .Select(osh => new OrderDto.OrderStatusHistoryItemDto
                            {
                                EnterDateTimeUtc = osh.EnterDateTimeUtc,
                                OrderStatus = osh.OrderStatus
                            })
                            .OrderBy(osh => osh.EnterDateTimeUtc)
                            .ThenBy(osh => osh.OrderStatus)
                            .ToList(),
                        UserId = o.User != null ? o.User.Id : null
                    })
                .ToArrayAsync(cancellationToken);

            var count = orders.Count();

            return new PaginatedResult<OrderDto>()
            {
                TotalElementsCount = count,
                PageElementsCount = result.Length,
                Elements = result,
                CurrentPageNumber = ordersQuery.Page,
                MaxPageNumber = (int)Math.Ceiling((decimal)count / ordersQuery.PageSize)
            };
        }

        private static IQueryable<Order> FilterByQueryType(IQueryable<Order> orders, OrdersQueryBase ordersQuery)
        {
            if (ordersQuery is OrdersByEmailQuery emailQuery)
            {
                return orders.Where(o => o.DeliveryDetails.Email == emailQuery.Email);
            }
            else if (ordersQuery is OrdersByUserQuery userQuery)
            {
                return orders.Where(o => o.User != default && o.User.Id == userQuery.UserId);
            }
            else if (ordersQuery is AllOrdersQuery)
            {
                return orders;
            }
            else
            {
                throw new ArgumentException("Invalid query type.");
            }
        }
    }
}
