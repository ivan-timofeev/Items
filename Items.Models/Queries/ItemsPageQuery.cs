using Items.Models.DataTransferObjects;

namespace Items.Models.Queries
{
    public sealed class ItemsPageQuery
    {
        public required int Page { get; init; }
        public required int PageSize { get; init; }
        public FilterDto? Filter { get; init; }
        public string? Sort { get; init; }
    }
}
