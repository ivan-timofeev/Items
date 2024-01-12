using System.Linq.Expressions;

namespace Items.Models
{
    public class PaginatedResult<T>
    {
        public required IEnumerable<T> Elements { get; init; }
        public required int CurrentPageNumber { get; init; }
        public required int MaxPageNumber { get; init; }
        public required int TotalElementsCount { get; init; }
        public required int PageElementsCount { get; init; }
    }
}
