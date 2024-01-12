using Items.Commands;
using Items.Data;
using Items.Models;
using Items.Models.DataTransferObjects.Item;
using Items.Models.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.EntityFrameworkCore;

namespace Items.Tests;

[TestClass]
public class UpdateItemCommandTests
{
    [TestMethod]
    public async Task ExecuteAsync_ItemExists_ShouldUpdateItem()
    {
        // Arrange
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Price = 50,
            Description = "Original Description",
            DisplayName = "Original Display name",
            AvailableQuantity = 0,
            ImageUrl = "Original image url",
            OverallRating = 0,
            Categories = new List<ItemCategory>()
        };

        var updateItemDto = new ItemDto
        {
            Id = item.Id,
            Price = 100,
            Description = "Updated Description",
            DisplayName = "Updated Display name",
            AvailableQuantity = 10,
            ImageUrl = "Updated image url",
            OverallRating = 0.5M,
            Categories = new string[] { "Books", "Electronics" }
        };

        var dbContextMock = new Mock<ItemsDbContext>();

        var items = new[] { item };

        dbContextMock
            .Setup(d => d.Items)
            .ReturnsDbSet(items);

        dbContextMock
            .Setup(d => d.ItemsCategory)
            .ReturnsDbSet(Array.Empty<ItemCategory>());

        var memoryCacheMock = new Mock<IMemoryCache>();

        var command = new UpdateItemCommand(item.Id, updateItemDto, dbContextMock.Object, memoryCacheMock.Object);


        // Act
        await command.ExecuteAsync(CancellationToken.None);


        // Assert
        Assert.AreEqual(updateItemDto.Price, items[0].Price);
        Assert.AreEqual(updateItemDto.Description, items[0].Description);
        Assert.AreEqual(updateItemDto.DisplayName, items[0].DisplayName);
        Assert.AreEqual(updateItemDto.AvailableQuantity, items[0].AvailableQuantity);
        Assert.AreEqual(updateItemDto.ImageUrl, items[0].ImageUrl);
        Assert.AreEqual(updateItemDto.OverallRating, items[0].OverallRating);
        CollectionAssert.AreEqual(updateItemDto.Categories.ToArray(), items[0].Categories.Select(c => c.DisplayName).ToArray());

        memoryCacheMock.Verify(
            m => m.Remove(Item.GetCacheKey(item.Id)),
            Times.Once);

        dbContextMock.Verify(
            d => d.SaveChangesAsync(It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [TestMethod]
    [ExpectedException(typeof(EntityNotFoundException))]
    public async Task ExecuteAsync_ItemNotExists_ShouldThrowException()
    {
        // Arrange
        var item = new Item
        {
            Id = Guid.NewGuid(),
            Price = 50,
            Description = "Original Description",
            DisplayName = "Original Display name",
            AvailableQuantity = 0,
            ImageUrl = "Original image url",
            OverallRating = 0,
            Categories = new List<ItemCategory>()
        };

        var updateItemDto = new ItemDto
        {
            Id = item.Id,
            Price = 100,
            Description = "Updated Description",
            DisplayName = "Updated Display name",
            AvailableQuantity = 10,
            ImageUrl = "Updated image url",
            OverallRating = 0.5M,
            Categories = new string[] { "Books", "Electronics" }
        };

        var dbContextMock = new Mock<ItemsDbContext>();

        dbContextMock
            .Setup(d => d.Items)
            .ReturnsDbSet(Array.Empty<Item>());

        var command = new UpdateItemCommand(item.Id, updateItemDto, dbContextMock.Object, Mock.Of<IMemoryCache>());


        // Act
        await command.ExecuteAsync(CancellationToken.None);
    }
}
