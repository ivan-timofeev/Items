using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Items.Models.DataTransferObjects.Order
{
    public class OrderDto
    {
        public required IList<OrderStatusHistoryItemDto> OrderStatusHistory { get; set; }
        public required IEnumerable<OrderItemDto> OrderItems { get; init; }
        public required DeliveryDetailsDto DeliveryDetails { get; init; }
        public required PaymentDto PaymentDetails { get; init; }
        public Guid? UserId { get; init; }
        public decimal TotalOrderPrice => OrderItems.Sum(x => x.TotalPrice);

        public class PaymentDto
        {

        }

        public class OrderItemDto
        {
            public required Guid ItemId { get; init; }
            public required string DisplayName { get; init; }
            public required string ImageUrl { get; init; }
            public required int Quantity { get; init; }
            public required decimal Price { get; init; }
            public decimal TotalPrice => Price * Quantity;
        }

        public class OrderStatusHistoryItemDto
        {
            [JsonConverter(typeof(JsonStringEnumConverter))]
            public required OrderStatus OrderStatus { get; init; }
            public required DateTime EnterDateTimeUtc { get; init; }
        }
    }
}
