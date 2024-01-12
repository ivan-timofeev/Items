namespace Items.Models;

public class Item
{
    public Guid Id { get; set; }
    public required string DisplayName { get; set; }
    public required int AvailableQuantity { get; set; }
    public required decimal Price { get; set; }
    public required IList<ItemCategory> Categories { get; set; }
    public required decimal OverallRating { get; set; }
    public required string Description { get; set; }
    public required string ImageUrl { get; set; }

    public Item()
    {
        Categories = new List<ItemCategory>();
    }

    public static string GetCacheKey(Guid id)
    {
        return $"DomainModels:Item:{id}";
    }
}
