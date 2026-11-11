using AutoMapper;
using FluentAssertions;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.SalesOrders.Commands;
using OsbErpLite.Application.SalesOrders.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.SalesOrders;

public class CompleteSalesOrderCommandHandlerTests
{
    private readonly Mock<ISalesOrderRepository> _salesOrderRepoMock;
    private readonly Mock<IProductRepository> _productRepoMock;
    private readonly Mock<IStockMovementRepository> _stockMovementRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CompleteSalesOrderCommandHandler _handler;

    public CompleteSalesOrderCommandHandlerTests()
    {
        _salesOrderRepoMock = new Mock<ISalesOrderRepository>();
        _productRepoMock = new Mock<IProductRepository>();
        _stockMovementRepoMock = new Mock<IStockMovementRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CompleteSalesOrderCommandHandler(
            _salesOrderRepoMock.Object,
            _productRepoMock.Object,
            _stockMovementRepoMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task Handle_ApprovedOrderWithSufficientStock_ShouldComplete()
    {
        var productId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Widget", SKU = "WDG-001", StockQuantity = 100 };
        var order = new SalesOrder
        {
            Id = orderId,
            Status = OrderStatus.Approved,
            Lines = new List<SalesOrderLine>
            {
                new() { ProductId = productId, Quantity = 10, UnitPrice = 50.00m }
            }
        };

        _salesOrderRepoMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _productRepoMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _stockMovementRepoMock.Setup(r => r.AddAsync(It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockMovement sm, CancellationToken _) => sm);
        _salesOrderRepoMock.Setup(r => r.UpdateAsync(order, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<SalesOrderDto>(order))
            .Returns(new SalesOrderDto { Id = orderId, Status = OrderStatus.Completed });

        var command = new CompleteSalesOrderCommand(orderId, Guid.NewGuid());
        var result = await _handler.Handle(command, CancellationToken.None);

        order.Status.Should().Be(OrderStatus.Completed);
        product.StockQuantity.Should().Be(90);
        _stockMovementRepoMock.Verify(
            r => r.AddAsync(It.Is<StockMovement>(sm =>
                sm.ProductId == productId &&
                sm.Quantity == 10 &&
                sm.MovementType == StockMovementType.Out),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InsufficientStock_ShouldThrowInvalidOperation()
    {
        var productId = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var product = new Product { Id = productId, Name = "Widget", SKU = "WDG-001", StockQuantity = 5 };
        var order = new SalesOrder
        {
            Id = orderId,
            Status = OrderStatus.Approved,
            Lines = new List<SalesOrderLine>
            {
                new() { ProductId = productId, Quantity = 10, UnitPrice = 50.00m }
            }
        };

        _salesOrderRepoMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);
        _productRepoMock.Setup(r => r.GetByIdAsync(productId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(product);

        var command = new CompleteSalesOrderCommand(orderId, Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Yetersiz stok*");
    }

    [Theory]
    [InlineData(OrderStatus.Draft)]
    [InlineData(OrderStatus.Pending)]
    [InlineData(OrderStatus.Rejected)]
    [InlineData(OrderStatus.Completed)]
    public async Task Handle_NonApprovedOrder_ShouldThrowInvalidOperation(OrderStatus status)
    {
        var orderId = Guid.NewGuid();
        var order = new SalesOrder { Id = orderId, Status = status, Lines = new List<SalesOrderLine>() };

        _salesOrderRepoMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(order);

        var command = new CompleteSalesOrderCommand(orderId, Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Approved*");
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ShouldThrowKeyNotFound()
    {
        var orderId = Guid.NewGuid();
        _salesOrderRepoMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SalesOrder?)null);

        var command = new CompleteSalesOrderCommand(orderId, Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task Handle_MultipleLines_ShouldDeductStockForEachProduct()
    {
        var productId1 = Guid.NewGuid();
        var productId2 = Guid.NewGuid();
        var orderId = Guid.NewGuid();
        var product1 = new Product { Id = productId1, Name = "A", SKU = "A-001", StockQuantity = 50 };
        var product2 = new Product { Id = productId2, Name = "B", SKU = "B-001", StockQuantity = 30 };
        var order = new SalesOrder
        {
            Id = orderId,
            Status = OrderStatus.Approved,
            Lines = new List<SalesOrderLine>
            {
                new() { ProductId = productId1, Quantity = 20, UnitPrice = 10m },
                new() { ProductId = productId2, Quantity = 15, UnitPrice = 20m }
            }
        };

        _salesOrderRepoMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>())).ReturnsAsync(order);
        _productRepoMock.Setup(r => r.GetByIdAsync(productId1, It.IsAny<CancellationToken>())).ReturnsAsync(product1);
        _productRepoMock.Setup(r => r.GetByIdAsync(productId2, It.IsAny<CancellationToken>())).ReturnsAsync(product2);
        _productRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Product>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _stockMovementRepoMock.Setup(r => r.AddAsync(It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((StockMovement sm, CancellationToken _) => sm);
        _salesOrderRepoMock.Setup(r => r.UpdateAsync(order, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<SalesOrderDto>(order)).Returns(new SalesOrderDto());

        await _handler.Handle(new CompleteSalesOrderCommand(orderId, Guid.NewGuid()), CancellationToken.None);

        product1.StockQuantity.Should().Be(30);
        product2.StockQuantity.Should().Be(15);
        _stockMovementRepoMock.Verify(
            r => r.AddAsync(It.IsAny<StockMovement>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}
