using Items.Abstractions.Commands.Handlers;
using Items.Abstractions.Services;
using Items.Data;
using Items.Models.DataTransferObjects.Order;
using Items.Models.Exceptions;
using Items.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Quartz;

namespace Items.Commands.Handlers
{
    internal sealed class CreateOrderCommandHandler : ICreateOrderCommandHandler
    {
        private readonly DbContextProvider _dbContextProvider;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly ISchedulerFactory _schedulerFactory;

        public CreateOrderCommandHandler(
            DbContextProvider dbContextProvider,
            IDateTimeProvider dateTimeProvider,
            ISchedulerFactory schedulerFactory)
        {
            _dbContextProvider = dbContextProvider;
            _dateTimeProvider = dateTimeProvider;
            _schedulerFactory = schedulerFactory;
        }

        public async Task ExecuteAsync(
            CreateOrderCommandBase createOrderCommand,
            CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);
            using var transaction = dbContext.Database.BeginTransaction(IsolationLevel.Serializable);

            var order = new Order
            {
                CreateDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                OrderItems = new List<OrderItem>(),
                OrderStatusHistory =
                    new List<OrderStatusHistoryItem>
                    {
                        new()
                        {
                            SerialNumber = 1,
                            EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                            OrderStatus = OrderStatus.Created
                        }
                    },
                DeliveryDetails =
                    new DeliveryDetails
                    {
                        Email = createOrderCommand.DeliveryDetails.Email,
                        FirstName = createOrderCommand.DeliveryDetails.FirstName,
                        LastName = createOrderCommand.DeliveryDetails.LastName,
                        CompanyName = createOrderCommand.DeliveryDetails.CompanyName
                    }
            };

            var requestedProducts = await GetOrderItemsAsync(dbContext, createOrderCommand, cancellationToken);

            ReserveProducts(order, createOrderCommand, requestedProducts);
            
            switch (createOrderCommand)
            {
                case CreateOrderFromAnonCommand createOrderFromAnon:
                    await CreateNewUserAsync(dbContext, order, createOrderFromAnon);
                    break;

                case CreateOrderFromUserCommand createOrderFromUser:
                    await UpdateUserAsync(dbContext, order, createOrderFromUser);
                    break;

                default:
                    throw new InvalidOperationException("Not supported create order command type.");
            }

            if (!string.IsNullOrWhiteSpace(createOrderCommand.Promocode))
            {
                ApplyPromocode(order, createOrderCommand.Promocode);
            }

            CreatePayment(dbContext, order);

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            var scheduler = await _schedulerFactory.GetScheduler();
            await scheduler.TriggerJob(new JobKey("ProcessCreatedPaymentsJob"));
        }

        private static async Task<IEnumerable<Item>> GetOrderItemsAsync(
            ItemsDbContext dbContext,
            CreateOrderCommandBase createOrderCommand,
            CancellationToken cancellationToken)
        {
            var result = await dbContext
                .Items
                .Where(i =>
                    createOrderCommand
                        .OrderItems
                        .Select(od => od.ItemId)
                        .ToArray()
                        .Contains(i.Id))
                .ToArrayAsync(cancellationToken);

            return result;
        }

        private void ReserveProducts(Order order, CreateOrderCommandBase createOrderCommand, IEnumerable<Item> requestedProducts)
        {
            foreach (var orderPosition in createOrderCommand.OrderItems)
            {
                var requestedProduct = requestedProducts
                    .Where(p => p.Id == orderPosition.ItemId)
                    .SingleOrDefault();

                if (requestedProduct == default)
                {
                    throw new BusinessException(
                        ListOfBusinessErrors.ProductNotFound,
                        new() { { "ProductId", orderPosition.ItemId.ToString() } });
                }

                if (requestedProduct.AvailableQuantity < orderPosition.Quantity)
                {
                    throw new BusinessException(
                        ListOfBusinessErrors.ProductsNotEnoughInStock,
                        new() { { "ProductId", orderPosition.ItemId.ToString() } });
                }

                requestedProduct.AvailableQuantity -= orderPosition.Quantity;

                order.OrderItems.Add(
                    new OrderItem
                    {
                        Item = requestedProduct,
                        Price = requestedProduct.Price,
                        Quantity = orderPosition.Quantity
                    });
            }

            order.OrderStatusHistory.Add(
                new OrderStatusHistoryItem
                {
                    SerialNumber = order.GetActualOrderStatusHistorySerialNumber() + 1,
                    EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                    OrderStatus = OrderStatus.ProductsReserved
                });
        }

        private async Task CreateNewUserAsync(ItemsDbContext dbContext, Order order, CreateOrderFromAnonCommand createOrderCommand)
        {
            var isUserWithProvidedEmailAlreadyExist = await dbContext
                .Users
                .Where(u => u.Email == createOrderCommand.DeliveryDetails.Email)
                .AnyAsync();

            if (isUserWithProvidedEmailAlreadyExist)
            {
                throw new BusinessException(
                    ListOfBusinessErrors.UserAlreadyExists,
                    new() { { "Email", createOrderCommand.DeliveryDetails.Email } });
            }

            var user = new User
            {
                Orders = new List<Order> { order },
                Email = createOrderCommand.DeliveryDetails.Email,
                FirstName = createOrderCommand.DeliveryDetails.FirstName,
                LastName = createOrderCommand.DeliveryDetails.LastName,
                PasswordHash = string.Empty
            };

            dbContext.Users.Add(user);

            order.OrderStatusHistory.Add(new()
            {
                SerialNumber = order.GetActualOrderStatusHistorySerialNumber() + 1,
                EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                OrderStatus = OrderStatus.UserCreated
            });
        }

        private async Task UpdateUserAsync(ItemsDbContext dbContext, Order order, CreateOrderFromUserCommand createOrderCommand)
        {
            var user = await dbContext
                .Users
                .Include(u => u.Orders)
                .Where(u => u.Id == createOrderCommand.UserId)
                .AsSingleQuery()
                .SingleOrDefaultAsync();

            if (user == default)
            {
                throw new BusinessException(
                    ListOfBusinessErrors.UserNotFound,
                    new() { { "UserId", createOrderCommand.UserId.ToString() } });
            }

            user.FirstName = createOrderCommand.DeliveryDetails.FirstName;
            user.LastName = createOrderCommand.DeliveryDetails.LastName;

            user.Orders.Add(order);

            order.OrderStatusHistory.Add(new()
            {
                SerialNumber = order.GetActualOrderStatusHistorySerialNumber() + 1,
                EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                OrderStatus = OrderStatus.UserUpdated
            });
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
                SerialNumber = order.GetActualOrderStatusHistorySerialNumber() + 1,
                EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                OrderStatus = OrderStatus.PromocodeApplied
            });
        }

        private void CreatePayment(ItemsDbContext dbContext, Order order)
        {
            var payment = new Payment()
            {
                Order = order,
                PaymentStatusHistory = new List<PaymentStatusHistoryItem>()
                {
                    new()
                    {
                        SerialNumber = 1,
                        PaymentStatus = PaymentStatus.WaitingForTransactionalOutbox,
                        EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc()
                    }
                }
            };

            order.OrderStatusHistory.Add(new()
            {
                SerialNumber = order.GetActualOrderStatusHistorySerialNumber() + 1,
                EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                OrderStatus = OrderStatus.CreatingPayment
            });

            dbContext.Payments.Add(payment);
        }
    }
}
