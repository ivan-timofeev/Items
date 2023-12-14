using Items.Data;
using Microsoft.EntityFrameworkCore;

namespace Items.Services;


public interface IUnitOfWorkFactory
{
    IUnitOfWork CreateUnitOfWork();
}

public class UnitOfWorkFactory : IUnitOfWorkFactory
{
    private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;

    public UnitOfWorkFactory(IDbContextFactory<ItemsDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }

    public IUnitOfWork CreateUnitOfWork()
    {
        return new UnitOfWork(_dbContextFactory.CreateDbContext());
    }
}
