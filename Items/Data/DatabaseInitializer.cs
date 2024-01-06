using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Items.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace Items.Data
{
    public interface IDatabaseInitializer
    {
        void InitializeDatabase();
    }

    internal sealed class DatabaseInitializer : IDatabaseInitializer
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;
        private readonly IDbContextFactory<ItemsDbContext> _dbContextFactory;

        public DatabaseInitializer(
            IUnitOfWorkFactory unitOfWorkFactory,
            IDbContextFactory<ItemsDbContext> dbContextFactory)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
            _dbContextFactory = dbContextFactory;
        }

        public void InitializeDatabase()
        {
            using var unitOfWork = _unitOfWorkFactory.CreateUnitOfWork();
            using var transaction = unitOfWork.BeginTransaction();

            ReduceCategories();

            if (!ShouldInitialize(unitOfWork))
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
                            .Select(c => new ItemCategory { DisplayName = c })
                            .ToList()
                    })
                .ToArray();

            foreach (var item in items)
            {
                unitOfWork.Items.AddNewItem(item);
            }

            unitOfWork.SaveChanges();
            transaction.Commit();
        }

        private void ReduceCategories()
        {
            using var dbContext = _dbContextFactory.CreateDbContext();
            var categories = dbContext.ItemsCategory.Include(ic => ic.Items).ToArray();

            foreach (var category in categories)
            {
                var categoryItems = dbContext
                    .ItemsCategory
                    .Where(ic => ic.DisplayName == category.DisplayName)
                    .SelectMany(ic => ic.Items)
                    .ToArray();

                var newCategory = new ItemCategory { DisplayName = category.DisplayName, Items = categoryItems };

                var categoriesToDelete = dbContext
                    .ItemsCategory
                    .Where(ic => ic.DisplayName == category.DisplayName)
                    .ToArray();

                dbContext.ItemsCategory.RemoveRange(categoriesToDelete);
                dbContext.SaveChanges();
                dbContext.ItemsCategory.Add(newCategory);
                dbContext.SaveChanges();
            }

        }

        private bool ShouldInitialize(IUnitOfWork unitOfWork)
        {
            return unitOfWork.Items.IsEmpty();
        }

        private IEnumerable<CreateItemDto> ReadItemsInitialFromDisk()
        {
            var pathToJson = Path.Combine("Data", "ItemsInitial.json");
            var json = File.ReadAllText(pathToJson, Encoding.UTF8);
            var items = JsonSerializer.Deserialize<IEnumerable<CreateItemDto>>(json)
                ?? throw new InvalidOperationException("Json must be of type IEnumerable<Item>.");
            return items;
        }
    }
}
