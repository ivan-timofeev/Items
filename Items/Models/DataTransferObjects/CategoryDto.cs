namespace Items.Models.DataTransferObjects
{
    public sealed class CategoryDto
    {
        public required string DisplayName { get; init; }
        public required int ProductsInCategory { get; init; }
    }
}
