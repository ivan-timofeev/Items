using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Text.Json;

namespace Items.Data
{
    public interface IDatabaseInitializer
    {
        void InitializeDatabase();
    }

    internal sealed class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;

        public DatabaseInitializer(
            IDbContextFactory<ItemsDbContext> dbContextFactory)
        {
            _dbContextFactory = dbContextFactory;
        }

        public void InitializeDatabase()
        {
            var dbContext = _dbContextFactory.CreateDbContext();

            if (dbContext.Items.Any())
                return;

            var items = ReadItemsInitialFromDisk()
                .Select(i =>
                    new Item
                    {
                        AvailableQuantity = i.Quantity,
                        Description = i.Description,
                        DisplayName = i.DisplayName,
                        ImageUrl = i.ImageUrl,
                        OverallRating = i.OverallRating,
                        Price = i.Price,
                        Categories = i
                            .Categories
                            .Select(c => GetOrCreateCategory(dbContext, c))
                            .ToList()
                    })
                .ToArray();

            foreach (var item in items)
            {
                dbContext.Items.Add(item);
            }

            dbContext.SaveChanges();
        }

        private static ItemCategory GetOrCreateCategory(ItemsDbContext dbContext, string displayName)
        {
            return dbContext
                .ItemsCategory
                .Where(ic => ic.DisplayName == displayName)
                .SingleOrDefault()
                ?? new ItemCategory { DisplayName = displayName };
        }

        private static IEnumerable<CreateItemDto> ReadItemsInitialFromDisk()
        {
            var pathToJson = Path.Combine("Data", "ItemsInitial.json");
            var json = File.ReadAllText(pathToJson, Encoding.UTF8);
            var items = JsonSerializer.Deserialize<IEnumerable<CreateItemDto>>(json)
                ?? throw new InvalidOperationException("Json must be of type IEnumerable<Item>.");
            return items;
        }
    }
}
