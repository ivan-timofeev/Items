using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Items.Models.Queries
{
    [JsonDerivedType(typeof(OrdersByEmailQuery), "email")]
    [JsonDerivedType(typeof(OrdersByUserQuery), "user")]
    [JsonDerivedType(typeof(AllOrdersQuery), "all")]
    public abstract class OrdersQueryBase
    {
        [Required, Range(1, 1000)]
        public required int Page { get; init; }

        [Required, Range(1, 25)]
        public required int PageSize { get; init; }
    }

    public sealed class OrdersByEmailQuery : OrdersQueryBase
    {
        [Required, MinLength(1), EmailAddress]
        public required string Email { get; init; }
    }

    public sealed class OrdersByUserQuery : OrdersQueryBase
    {
        [Required]
        public required Guid UserId { get; init; }
    }

    public sealed class AllOrdersQuery : OrdersQueryBase
    {
    }
}
