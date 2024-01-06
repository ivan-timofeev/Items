using System.ComponentModel.DataAnnotations;

namespace Items.Models.DataTransferObjects.Item;

public record CreateItemDto
(
    [Required] string DisplayName,
    [Required] int Quantity,
    [Required] decimal Price,
    [Required] IEnumerable<string> Categories,
    [Required] string Description,
    [Required] string ImageUrl,
    [Required] decimal OverallRating
);
