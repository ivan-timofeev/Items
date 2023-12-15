using Items.Models.DataTransferObjects.CreateOrder;

namespace Items.Services;

public interface IReserveItemsRequestProcessor
{
    void ProcessReserveItemsRequest(ReserveItemsRequest reserveItemsRequest);
}

public class ReserveItemsRequestProcessor : IReserveItemsRequestProcessor
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IOrdersMicroserviceApiClient _ordersMicroserviceApiClient;

    public ReserveItemsRequestProcessor(
        IUnitOfWorkFactory unitOfWorkFactory,
        IOrdersMicroserviceApiClient ordersMicroserviceApiClient)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _ordersMicroserviceApiClient = ordersMicroserviceApiClient;
    }
    
    public void ProcessReserveItemsRequest(ReserveItemsRequest reserveItemsRequest)
    {
        using var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork();
        using var transaction = unitOfWork.BeginTransaction();

        var requestedItemsIds = reserveItemsRequest
            .RequestedItems
            .Select(i => i.ItemId)
            .ToArray();

        var itemIds = reserveItemsRequest
            .RequestedItems
            .Select(r => r.ItemId)
            .ToArray();

        var foundItems = unitOfWork
            .Items
            .GetItems(itemIds);

        int a = 102;
        if (a == 102)
            throw new Exception("Test ci/cd");

        if (foundItems.Count != requestedItemsIds.Length)
        {
            MakeErrorResponse(
                reserveItemsRequest.OrderId,
                "One or more of requested items not found.");

            return;
        }

        foreach (var requestedItem in reserveItemsRequest.RequestedItems)
        {
            var foundItem = foundItems
                .Where(i => i.Id == requestedItem.ItemId)
                .Single();

            if (foundItem.AvailableQuantity < requestedItem.RequestedQuantity)
            {
                MakeErrorResponse(
                    reserveItemsRequest.OrderId,
                    "One or more requested items are not enough in stock");

                return;
            }

            unitOfWork.Items.UpdateItemQuantity(
                foundItem,
                foundItem.AvailableQuantity - requestedItem.RequestedQuantity);
        }
        
        unitOfWork.SaveChanges();
        transaction.Commit();

        MakeSuccessResponse(reserveItemsRequest.OrderId);
    }

    private void MakeErrorResponse(Guid orderId, string message)
    {
        var response = new ReserveItemsResponse
        {
            TransactionalId = Guid.NewGuid(),
            OrderId = orderId,
            Status = ReserveItemsResponseStatusEnum.Error,
            Message = message
        };

        _ordersMicroserviceApiClient.MakeResponseForReserveItemsRequest(response);
    }

    private void MakeSuccessResponse(Guid orderId)
    {
        var response = new ReserveItemsResponse
        {
            TransactionalId = Guid.NewGuid(),
            OrderId = orderId,
            Status = ReserveItemsResponseStatusEnum.Success
        };

        _ordersMicroserviceApiClient.MakeResponseForReserveItemsRequest(response);
    }
}
