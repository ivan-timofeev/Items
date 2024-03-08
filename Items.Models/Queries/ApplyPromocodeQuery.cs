using System.ComponentModel.DataAnnotations;

namespace Items.Models.Queries
{
    public sealed class ApplyPromocodeQuery
    {
        [Required, MinLength(1)]
        public required IEnumerable<CartItem> CartItems { get; init; }

        [Required]
        public required string Promocode { get; init; }

        public sealed class CartItem
        {
            [Required]
            public required Guid ItemId { get; init; }
        }
    }
}
