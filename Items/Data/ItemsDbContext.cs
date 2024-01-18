using Microsoft.EntityFrameworkCore;
using Items.Models;

namespace Items.Data;

public class ItemsDbContext : DbContext
{
    public virtual DbSet<Item> Items { get; set; }
    public virtual DbSet<ItemCategory> ItemsCategory { get; set; }
    public virtual DbSet<User> Users { get; set; }

    public ItemsDbContext(DbContextOptions options) : base(options)
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();
        Items = Set<Item>();
        ItemsCategory = Set<ItemCategory>();
        Users = Set<User>();
    }

    protected ItemsDbContext() { }
}

public delegate Task<ItemsDbContext> DbContextProvider(CancellationToken cancellationToken);
