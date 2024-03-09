using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Items.Models.DataTransferObjects.Order
{
    [JsonDerivedType(typeof(CreateOrderFromAnonCommand), "anon")]
    [JsonDerivedType(typeof(CreateOrderFromUserCommand), "user")]
    public abstract class CreateOrderCommandBase
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

    public sealed class CreateOrderFromUserCommand : CreateOrderCommandBase
    {
        public required Guid UserId { get; init; }
    }

    public sealed class CreateOrderFromAnonCommand : CreateOrderCommandBase
    {

    }
}
