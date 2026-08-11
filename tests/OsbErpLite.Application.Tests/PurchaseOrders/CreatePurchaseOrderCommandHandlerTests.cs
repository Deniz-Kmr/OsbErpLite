using AutoMapper;
using FluentAssertions;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.PurchaseOrders.Commands;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.PurchaseOrders;

public class CreatePurchaseOrderCommandHandlerTests
{
    private readonly Mock<IPurchaseOrderRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreatePurchaseOrderCommandHandler _handler;

    public CreatePurchaseOrderCommandHandlerTests()
    {
        _repositoryMock = new Mock<IPurchaseOrderRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreatePurchaseOrderCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldCalculateTotalAmountCorrectly()
    {
        var lines = new List<CreatePurchaseOrderLineDto>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 5, UnitPrice = 100.00m },
            new() { ProductId = Guid.NewGuid(), Quantity = 3, UnitPrice = 50.00m }
        };
        var command = new CreatePurchaseOrderCommand(Guid.NewGuid(), DateTime.UtcNow, lines);

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseOrder po, CancellationToken _) =>
            {
                po.Id = Guid.NewGuid();
                return po;
            });

        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>()))
            .Returns((PurchaseOrder po) => new PurchaseOrderDto
            {
                Id = po.Id,
                TotalAmount = po.TotalAmount,
                Status = po.Status
            });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.TotalAmount.Should().Be(650.00m);
    }

    [Fact]
    public async Task Handle_ShouldSetStatusToDraft()
    {
        var lines = new List<CreatePurchaseOrderLineDto>
        {
            new() { ProductId = Guid.NewGuid(), Quantity = 1, UnitPrice = 10.00m }
        };
        var command = new CreatePurchaseOrderCommand(Guid.NewGuid(), DateTime.UtcNow, lines);

        PurchaseOrder? capturedEntity = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()))
            .Callback<PurchaseOrder, CancellationToken>((po, _) => capturedEntity = po)
            .ReturnsAsync((PurchaseOrder po, CancellationToken _) => po);

        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>()))
            .Returns(new PurchaseOrderDto());

        await _handler.Handle(command, CancellationToken.None);

        capturedEntity.Should().NotBeNull();
        capturedEntity!.Status.Should().Be(OrderStatus.Draft);
    }

    [Fact]
    public async Task Handle_WithEmptyLines_ShouldSetTotalAmountToZero()
    {
        var command = new CreatePurchaseOrderCommand(Guid.NewGuid(), DateTime.UtcNow, new List<CreatePurchaseOrderLineDto>());

        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseOrder po, CancellationToken _) => po);

        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>()))
            .Returns((PurchaseOrder po) => new PurchaseOrderDto { TotalAmount = po.TotalAmount });

        var result = await _handler.Handle(command, CancellationToken.None);

        result.TotalAmount.Should().Be(0m);
    }

    [Fact]
    public async Task Handle_ShouldMapLinesToEntity()
    {
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var lines = new List<CreatePurchaseOrderLineDto>
        {
            new() { ProductId = productId1, Quantity = 2, UnitPrice = 25.00m },
            new() { ProductId = productId2, Quantity = 4, UnitPrice = 30.00m }
        };
        var command = new CreatePurchaseOrderCommand(Guid.NewGuid(), DateTime.UtcNow, lines);

        PurchaseOrder? capturedEntity = null;
        _repositoryMock.Setup(r => r.AddAsync(It.IsAny<PurchaseOrder>(), It.IsAny<CancellationToken>()))
            .Callback<PurchaseOrder, CancellationToken>((po, _) => capturedEntity = po)
            .ReturnsAsync((PurchaseOrder po, CancellationToken _) => po);

        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(It.IsAny<PurchaseOrder>()))
            .Returns(new PurchaseOrderDto());

        await _handler.Handle(command, CancellationToken.None);

        capturedEntity!.Lines.Should().HaveCount(2);
        capturedEntity.Lines.First().ProductId.Should().Be(productId1);
        capturedEntity.Lines.Last().ProductId.Should().Be(productId2);
    }
}
