using Items.Models;
using Items.Models.DataTransferObjects.CreateOrder;
using Items.Models.DataTransferObjects.Item;
using Items.Services;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace Items.Tests;

[TestClass]
public sealed class ReserveItemsRequestProcessorTests
{
    private readonly Mock<IItemsRepository> _itemsRepositoryMock;
    private readonly Mock<IUnitOfWorkFactory> _unitOfWorkFactoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IDbContextTransaction> _transactionMock;
    private readonly Mock<IOrdersMicroserviceApiClient> _ordersApiClientMock;
    
    public ReserveItemsRequestProcessorTests()
    {
        _itemsRepositoryMock = new Mock<IItemsRepository>();
        _transactionMock = new Mock<IDbContextTransaction>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _unitOfWorkFactoryMock = new Mock<IUnitOfWorkFactory>();
        _ordersApiClientMock = new Mock<IOrdersMicroserviceApiClient>();
        
        _unitOfWorkMock
            .Setup(u => u.Items)
            .Returns(_itemsRepositoryMock.Object);
        _unitOfWorkMock
            .Setup(u => u.BeginTransaction())
            .Returns(_transactionMock.Object);

        _unitOfWorkFactoryMock
            .Setup(f => f.CreateUnitOfWork())
            .Returns(_unitOfWorkMock.Object);
    }
    
    [DataTestMethod]
    [DataRow(1)]
    [DataRow(100)]
    public void ProcessReserveItemsRequest_HappyPath_ShouldMakeSuccessResponse(int requestedQuantity)
    {
        // Arrange
        var item = new Item
        {
            Id = Guid.NewGuid(),
            DisplayName = "some-display-name",
            AvailableQuantity = 100,
            Price = 100
        };
        
        _itemsRepositoryMock
            .Setup(r => r.GetItems(new[] { item.Id }))
            .Returns(new[] { item });

        var processor = new ReserveItemsRequestProcessor(
            _unitOfWorkFactoryMock.Object,
            _ordersApiClientMock.Object);
        
        var request = new ReserveItemsRequest
        {
            TransactionalId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            RequestedItems = new[]
            {
                new RequestedItemDto
                {
                    ItemId = item.Id,
                    RequestedQuantity = requestedQuantity
                }
            }
        };

        // Act
        processor.ProcessReserveItemsRequest(request);

        // Assert
        var expectedQuantity = item.AvailableQuantity - requestedQuantity;

        _itemsRepositoryMock.Verify(
            r => r.UpdateItemQuantity(item, expectedQuantity),
            Times.Once);

        _unitOfWorkMock.Verify(
            u => u.SaveChanges(),
            Times.Once);

        _transactionMock.Verify(
            t => t.Commit(),
            Times.Once);
        
        _ordersApiClientMock.Verify(c =>
                c.MakeResponseForReserveItemsRequest(
                    It.Is<ReserveItemsResponse>(response =>
                        response.Status == ReserveItemsResponseStatusEnum.Success)),
            Times.Once);
    }

    [TestMethod]
    public void ProcessReserveItemsRequest_ItemNotFound_ShouldMakeErrorResponse()
    {
        // Arrange
        _itemsRepositoryMock
            .Setup(r => r.GetItems(It.IsAny<IReadOnlyCollection<Guid>>()))
            .Returns(Array.Empty<Item>());

        var processor = new ReserveItemsRequestProcessor(
            _unitOfWorkFactoryMock.Object,
            _ordersApiClientMock.Object);
        
        var request = new ReserveItemsRequest
        {
            TransactionalId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            RequestedItems = new[]
            {
                new RequestedItemDto
                {
                    ItemId = Guid.NewGuid(),
                    RequestedQuantity = 1
                }
            }
        };

        // Act
        processor.ProcessReserveItemsRequest(request);

        // Assert
        _transactionMock.Verify(
            t => t.Commit(),
            Times.Never);
        
        _ordersApiClientMock.Verify(c =>
                c.MakeResponseForReserveItemsRequest(
                    It.Is<ReserveItemsResponse>(response =>
                        response.Status == ReserveItemsResponseStatusEnum.Error
                        && response.Message!.Contains("not found"))),
            Times.Once);
    }

    [TestMethod]
    public void ProcessReserveItemsRequest_ItemNotHaveEnoughQuantityInStock_ShouldMakeErrorResponse()
    {
        // Arrange
        var item = new Item
        {
            Id = Guid.NewGuid(),
            DisplayName = "some-display-name",
            AvailableQuantity = 1,
            Price = 100
        };

        _itemsRepositoryMock
            .Setup(r => r.GetItems(new[] { item.Id }))
            .Returns(new[] { item });

        var processor = new ReserveItemsRequestProcessor(
            _unitOfWorkFactoryMock.Object,
            _ordersApiClientMock.Object);

        var request = new ReserveItemsRequest
        {
            TransactionalId = Guid.NewGuid(),
            OrderId = Guid.NewGuid(),
            RequestedItems = new[]
            {
                new RequestedItemDto
                {
                    ItemId = item.Id,
                    RequestedQuantity = 1000
                }
            }
        };

        // Act
        processor.ProcessReserveItemsRequest(request);

        // Assert
        _transactionMock.Verify(
            t => t.Commit(),
            Times.Never);
        
        _ordersApiClientMock.Verify(c =>
                c.MakeResponseForReserveItemsRequest(
                    It.Is<ReserveItemsResponse>(response =>
                        response.Status == ReserveItemsResponseStatusEnum.Error
                        && response.Message!.Contains("not enough"))),
            Times.Once);
    }
}
