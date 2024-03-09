using Microsoft.EntityFrameworkCore;
using Items.Models;

namespace Items.Data;

public class ItemsDbContext : DbContext
{
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<ItemCategory> ItemsCategory { get; set; }
    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderItem> OrderItems { get; set; }
    public virtual DbSet<OrderStatusHistoryItem> OrderStatusHistoryItems { get; set; }
    public virtual DbSet<DeliveryDetails> DeliveryDetails { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<PaymentStatusHistoryItem> PaymentStatusHistoryItems { get; set; }

    public ItemsDbContext(DbContextOptions options) : base(options)
    {
        Items = Set<Item>();
        ItemsCategory = Set<ItemCategory>();
        Users = Set<User>();
        Orders = Set<Order>();
        OrderItems = Set<OrderItem>();
        OrderStatusHistoryItems = Set<OrderStatusHistoryItem>();
        DeliveryDetails = Set<DeliveryDetails>();
        Payments = Set<Payment>();
        PaymentStatusHistoryItems = Set<PaymentStatusHistoryItem>();
    }

    protected ItemsDbContext() { }
}

public delegate Task<ItemsDbContext> DbContextProvider(CancellationToken cancellationToken);
