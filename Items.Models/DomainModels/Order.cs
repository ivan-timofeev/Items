using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Items.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public User? User { get; set; }
        public required DeliveryDetails DeliveryDetails { get; set; }
        public required DateTime CreateDateTimeUtc { get; set; }
        public required IList<OrderItem> OrderItems { get; set; }
            = new List<OrderItem>();
        public required IList<OrderStatusHistoryItem> OrderStatusHistory { get; set; }
            = new List<OrderStatusHistoryItem>();
    }

    public class DeliveryDetails
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public required string Email { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public string? CompanyName { get; set; }
    }

    public class OrderItem
    {
        public Guid Id { get; set; }
        public Guid ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public required int Quantity { get; set; }
        public required decimal Price { get; set; }
    }

    public class OrderStatusHistoryItem
    {
        public Guid Id { get; set; }
        public required OrderStatus OrderStatus { get; init; }
        public required DateTime EnterDateTimeUtc { get; init; }

    }

    public enum OrderStatus
    {
        Canceled,
        Created,
        WaitingForPayment,
        InDelivery,
        Completed,
        PromocodeApplied
    }
}
