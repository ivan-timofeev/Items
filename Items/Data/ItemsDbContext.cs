using Microsoft.EntityFrameworkCore;
using Items.Models;

namespace Items.Data;

public sealed class ItemsDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }
    public DbSet<ItemCategory> ItemsCategory { get; set; }

    public ItemsDbContext(DbContextOptions options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
        Items = Set<Item>();
        ItemsCategory = Set<ItemCategory>();
    }
}
