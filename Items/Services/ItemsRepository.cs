using Items.Data;
using Items.Models;

namespace Items.Services;

public interface IItemsRepository
{
    IReadOnlyCollection<Item> GetItems(IReadOnlyCollection<Guid> ids);
    void UpdateItemQuantity(Item item, int quantity);
}

public class ItemsRepository : IItemsRepository
{
    private readonly ItemsDbContext _itemsDbContext;

    public ItemsRepository(ItemsDbContext itemsDbContext)
    {
        _itemsDbContext = itemsDbContext;
    }
    
    public IReadOnlyCollection<Item> GetItems(IReadOnlyCollection<Guid> ids)
    {
        return _itemsDbContext
            .Items
            .Where(i => ids.Contains(i.Id))
            .ToArray();
    }

    public void UpdateItemQuantity(Item item, int quantity)
    {
        item.AvailableQuantity = quantity;
    }
}
