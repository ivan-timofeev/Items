namespace Items.Models.Queries
{
    public sealed class ItemListQuery
    {
        public required IReadOnlyCollection<Guid> ItemsIds { get; init; }
    }
}
