using Microsoft.EntityFrameworkCore;
using Items.Models;

namespace Items.Data;

public sealed class ItemsDbContext : DbContext
{
    public DbSet<Item> Items { get; set; }

    public ItemsDbContext(DbContextOptions options) : base(options)
    {
        Database.EnsureCreated();
        Items = Set<Item>();
    }
}
