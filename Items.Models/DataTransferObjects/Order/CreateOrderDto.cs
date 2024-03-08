using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Items.Models.DataTransferObjects.Order
{
    [JsonDerivedType(typeof(CreateAnonymOrderDto), "anon")]
    [JsonDerivedType(typeof(CreateUserOrder), "user")]
    public abstract class CreateOrderBase
    {
        [Required, MinLength(1)]
        public required IEnumerable<OrderItemDto> OrderItems { get; init; }

        [Required]
        public required DeliveryDetailsDto DeliveryDetails { get; init; }

        public string? Promocode { get; init; }

        public class OrderItemDto
        {
            public required Guid ItemId { get; init; }
            public required int Quantity { get; init; }
        }
    }

    public sealed class CreateUserOrder : CreateOrderBase
    {
        public required Guid UserId { get; init; }
    }

    public sealed class CreateAnonymOrderDto : CreateOrderBase
    {
        public required bool ShouldCreateNewAccount { get; init; }
    }
}
