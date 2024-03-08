namespace Items.Models.DataTransferObjects
{
    public sealed class ApplyPromocodeResponse
    {
        public required IEnumerable<CartItem> CartItems { get; init; }

        public sealed class CartItem
        {
            public required Guid ItemId { get; init; }
            public required decimal OldPrice { get; init; }
            public required decimal NewPrice { get; init; }
            public required decimal DiscountPercentage { get; init; }
        }
    }
}
