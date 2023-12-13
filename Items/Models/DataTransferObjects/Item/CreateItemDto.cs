using System.ComponentModel.DataAnnotations;

namespace Items.Models.DataTransferObjects.Item;

public record CreateItemDto
(
    [Required] string DisplayName,
    [Required] int Quantity,
    [Required] decimal Price
);
