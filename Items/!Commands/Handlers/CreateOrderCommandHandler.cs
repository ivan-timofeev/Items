using Items.Abstractions.Commands.Handlers;
using Items.Abstractions.Services;
using Items.Data;
using Items.Models.DataTransferObjects.Order;
using Items.Models.Exceptions;
using Items.Models;
using Microsoft.EntityFrameworkCore;
using Items.Helpers;
using System.Data;

namespace Items.Commands.Handlers
{
    internal sealed class CreateOrderCommandHandler : ICreateOrderCommandHandler
    {
        private readonly DbContextProvider _dbContextProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public CreateOrderCommandHandler(
            DbContextProvider dbContextProvider,
            IDateTimeProvider dateTimeProvider)
        {
            _dbContextProvider = dbContextProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task ExecuteAsync(
            CreateOrderBase createOrderDto,
            CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);
            using var transaction = dbContext.Database.BeginTransaction(IsolationLevel.Serializable);
            var utcNow = _dateTimeProvider.GetCurrentDateTimeUtc();

            var items = await dbContext
                .Items
                .Where(i =>
                    createOrderDto
                        .OrderItems
                        .Select(od => od.ItemId)
                        .ToArray()
                        .Contains(i.Id))
                .ToArrayAsync(cancellationToken);

            var order = new Order
            {
                CreateDateTimeUtc = utcNow,
                OrderItems = createOrderDto
                    .OrderItems
                    .Select(oi =>
                        new OrderItem
                        {
                            ItemId = oi.ItemId,
                            Quantity = oi.Quantity,
                            Price = items.Single(i => i.Id == oi.ItemId).Price
                        })
                    .ToList(),
                OrderStatusHistory = new List<OrderStatusHistoryItem>
                {
                    new()
                    {
                        EnterDateTimeUtc = utcNow,
                        OrderStatus = OrderStatus.Created
                    },
                    new()
                    {
                        EnterDateTimeUtc = utcNow,
                        OrderStatus = OrderStatus.WaitingForPayment
                    }
                },
                DeliveryDetails = new DeliveryDetails
                {
                    Email = createOrderDto.DeliveryDetails.Email,
                    FirstName = createOrderDto.DeliveryDetails.FirstName,
                    LastName = createOrderDto.DeliveryDetails.LastName,
                    CompanyName = createOrderDto.DeliveryDetails.CompanyName
                }
            };

            dbContext.Orders.Add(order);

            foreach (var item in items)
            {
                var requestedItem = createOrderDto
                    .OrderItems
                    .Where(oi => oi.ItemId == item.Id)
                    .Single();

                if (item.AvailableQuantity < requestedItem.Quantity)
                    throw new BusinessException(
                        "Not enough in stock.",
                        new() { { "ItemId", item.Id.ToString() } });

                item.AvailableQuantity -= requestedItem.Quantity;
            }

            if (createOrderDto is CreateAnonymOrderDto createAnonymOrderDto
                && createAnonymOrderDto.ShouldCreateNewAccount)
            {
                var isUserWithProvidedEmailAlreadyExist = dbContext
                    .Users
                    .Where(u => u.Email == createOrderDto.DeliveryDetails.Email)
                    .Any();

                if (isUserWithProvidedEmailAlreadyExist)
                    throw new BusinessException("User with provided email already exists.");

                var user = new User
                {
                    Orders = new List<Order> { order },
                    Email = createOrderDto.DeliveryDetails.Email,
                    FirstName = createOrderDto.DeliveryDetails.FirstName,
                    LastName = createOrderDto.DeliveryDetails.LastName,
                    PasswordHash = string.Empty
                };

                dbContext.Users.Add(user);
            }
            else if (createOrderDto is CreateUserOrder createUserOrder)
            {
                var user = await dbContext
                    .Users
                    .Where(u => u.Id == createUserOrder.UserId)
                    .SingleOrThrowNotFoundExceptionAsync(
                        nameof(User),
                        createUserOrder.UserId.ToString()!,
                        cancellationToken);

                user.Orders.Add(order);
            }

            if (!string.IsNullOrWhiteSpace(createOrderDto.Promocode))
            {
                ApplyPromocode(order, createOrderDto.Promocode);
            }

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }

        private void ApplyPromocode(Order order, string promocode)
        {
            if (promocode != "test")
                return;

            foreach (var orderItem in order.OrderItems)
            {
                orderItem.Price = 1;
            }

            order.OrderStatusHistory.Add(new()
            {
                EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                OrderStatus = OrderStatus.PromocodeApplied
            });
        }
    }
}
