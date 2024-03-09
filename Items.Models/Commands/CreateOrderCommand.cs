#nullable disable
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Items.Models.DataTransferObjects.Order;

namespace Items.Models.Commands
{
    [JsonDerivedType(typeof(CreateOrderFromAnonCommand), "anon")]
    [JsonDerivedType(typeof(CreateOrderFromUserCommand), "user")]
    public abstract class CreateOrderCommandBase
    {
        [Required, MinLength(1)]
        public IEnumerable<OrderItemDto> OrderItems { get; init; }

        [Required]
        public DeliveryDetailsDto DeliveryDetails { get; init; }

        public string Promocode { get; init; }

        public class OrderItemDto
        {
            public required Guid ItemId { get; init; }
            public required int Quantity { get; init; }
        }
    }

    public sealed class CreateOrderFromUserCommand : CreateOrderCommandBase
    {
        [Required]
        public Guid UserId { get; init; }
    }

    public sealed class CreateOrderFromAnonCommand : CreateOrderCommandBase
    {

    }
}
