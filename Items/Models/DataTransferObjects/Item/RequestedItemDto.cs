namespace Items.Models.DataTransferObjects.Item;

public class RequestedItemDto
{
    public required Guid ItemId { get; init; }
    public required int RequestedQuantity { get; init; }
}
