using Items.Abstractions.Commands.Handlers;
using Items.Abstractions.Services;
using Items.Data;
using Items.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;
using Serilog;
using Items.Models.Commands;

namespace Items.Commands.Handlers
{
    internal sealed class ProcessCreatedPaymentsCommandHandler : IProcessCreatedPaymentsCommandHandler
    {
        private readonly DbContextProvider _dbContextProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ProcessCreatedPaymentsCommandHandler(
            DbContextProvider dbContextProvider,
            IDateTimeProvider dateTimeProvider)
        {
            _dbContextProvider = dbContextProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task ExecuteAsync(ProcessCreatedPaymentsCommand command, CancellationToken cancellationToken)
        {
            var dbContext = await _dbContextProvider.Invoke(cancellationToken);

            Guid[] paymentsToProcess;

            using (dbContext)
            {
                paymentsToProcess = await dbContext
                   .Payments
                   .Include(p => p.PaymentStatusHistory)
                   .FilterByActualPaymentStatus(PaymentStatus.WaitingForTransactionalOutbox)
                   .AsSingleQuery()
                   .AsNoTracking()
                   .Select(p => p.Id)
                   .Take(command.TakeLimit)
                   .ToArrayAsync(cancellationToken);
            }

            if (!paymentsToProcess.Any())
                return;

            var parallelOptions = new ParallelOptions
            {
                MaxDegreeOfParallelism = command.MaxDegreeOfParallelism,
                CancellationToken = cancellationToken
            };

            await Parallel.ForEachAsync(
                paymentsToProcess,
                parallelOptions,
                async (paymentId, cancellationToken) => 
                {
                    var updateDbContext = await _dbContextProvider.Invoke(cancellationToken);
                    using var transaction = updateDbContext.Database.BeginTransaction(IsolationLevel.Serializable);

                    var payment = updateDbContext
                        .Payments
                        .Include(p => p.PaymentStatusHistory)
                        .Include(p => p.Order)
                        .ThenInclude(o => o.OrderStatusHistory)
                        .Where(p => p.Id == paymentId)
                        .AsSingleQuery()
                        .SingleOrDefault();

                    if (payment == default)
                        return;

                    payment.PaymentKey = await new PaymentService().IssuePaymentAsync(payment.Order, cancellationToken);

                    payment.PaymentStatusHistory.Add(
                        new PaymentStatusHistoryItem
                        {
                            SerialNumber = payment.GetActualSerialNumber() + 1,
                            EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                            PaymentStatus = PaymentStatus.WaitingForPayment
                        });

                    payment.Order.OrderStatusHistory.Add(
                        new OrderStatusHistoryItem
                        {
                            SerialNumber = payment.Order.GetActualOrderStatusHistorySerialNumber() + 1,
                            EnterDateTimeUtc = _dateTimeProvider.GetCurrentDateTimeUtc(),
                            OrderStatus = OrderStatus.WaitingForPayment
                        });

                    await updateDbContext.SaveChangesAsync(cancellationToken);
                    await transaction.CommitAsync(cancellationToken);
                });
        }
    }

    internal sealed class PaymentService
    {
        public Task<string> IssuePaymentAsync(Order order, CancellationToken cancellationToken)
        {
            Log.Logger.Information("PAYMENT CREATED. ORDER - ", order.Id);
            return Task.FromResult($"TEST,{order.Id}");
        }
    }

    public static class Test
    {
        public static IQueryable<Payment> FilterByActualPaymentStatus(this IQueryable<Payment> query, PaymentStatus paymentStatus)
        {
            return query.Where(payments =>
                payments
                    .PaymentStatusHistory
                    .OrderByDescending(psh => psh.SerialNumber)
                    .First()
                    .PaymentStatus == paymentStatus);
        }
    }
}
