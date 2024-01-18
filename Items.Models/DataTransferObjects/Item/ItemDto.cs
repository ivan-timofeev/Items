using System.ComponentModel.DataAnnotations;

namespace Items.Models.DataTransferObjects.Item;

public class ItemDto
{
    public required Guid Id { get; init; }
    public required string DisplayName { get; init; }
    public required int AvailableQuantity { get; init; }
    public required decimal Price { get; init; }
    public required IEnumerable<string> Categories { get; init; }
    public required string Description { get; init; }
    public required string ImageUrl { get; init; }
    public required decimal OverallRating { get; init; }
}
