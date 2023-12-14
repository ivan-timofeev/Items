using Items.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Items.Services;

public interface IUnitOfWork : IDisposable
{
    IItemsRepository Items { get; }
    IDbContextTransaction BeginTransaction();
    void SaveChanges();
}

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ItemsDbContext _itemsDbContext;
    public IItemsRepository Items { get; }

    public UnitOfWork(ItemsDbContext itemsDbContext)
    {
        _itemsDbContext = itemsDbContext;
        Items = new ItemsRepository(itemsDbContext);
    }
    
    public void Dispose()
    {
        _itemsDbContext.Dispose();
    }

    public IDbContextTransaction BeginTransaction()
    {
        return _itemsDbContext.Database.BeginTransaction();
    }

    public void SaveChanges()
    {
        _itemsDbContext.SaveChanges();
    }
}
