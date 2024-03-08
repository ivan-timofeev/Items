using Items.Models.DataTransferObjects.Order;

namespace Items.Abstractions.Commands.Handlers
{
    public interface ICreateOrderCommandHandler
    {
        Task ExecuteAsync(
            CreateOrderBase createOrderDto,
            CancellationToken cancellationToken);
    }
}
