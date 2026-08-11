using AutoMapper;
using FluentAssertions;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.PurchaseOrders.Commands;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.PurchaseOrders;

public class ApprovePurchaseOrderCommandHandlerTests
{
    private readonly Mock<IPurchaseOrderRepository> _repositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly ApprovePurchaseOrderCommandHandler _handler;

    public ApprovePurchaseOrderCommandHandlerTests()
    {
        _repositoryMock = new Mock<IPurchaseOrderRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new ApprovePurchaseOrderCommandHandler(_repositoryMock.Object, _mapperMock.Object);
    }

    [Theory]
    [InlineData(OrderStatus.Draft)]
    [InlineData(OrderStatus.Pending)]
    public async Task Handle_DraftOrPendingOrder_ShouldApprove(OrderStatus initialStatus)
    {
        var orderId = Guid.NewGuid();
        var approverEmployeeId = Guid.NewGuid();
        var entity = new PurchaseOrder { Id = orderId, Status = initialStatus };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);
        _repositoryMock.Setup(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _mapperMock.Setup(m => m.Map<PurchaseOrderDto>(entity))
            .Returns(new PurchaseOrderDto { Id = orderId, Status = OrderStatus.Approved });

        var command = new ApprovePurchaseOrderCommand(orderId, approverEmployeeId);
        var result = await _handler.Handle(command, CancellationToken.None);

        entity.Status.Should().Be(OrderStatus.Approved);
        entity.ApprovedByEmployeeId.Should().Be(approverEmployeeId);
        _repositoryMock.Verify(r => r.UpdateAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData(OrderStatus.Approved)]
    [InlineData(OrderStatus.Rejected)]
    [InlineData(OrderStatus.Completed)]
    public async Task Handle_NonDraftOrPendingOrder_ShouldThrowInvalidOperation(OrderStatus invalidStatus)
    {
        var orderId = Guid.NewGuid();
        var entity = new PurchaseOrder { Id = orderId, Status = invalidStatus };

        _repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var command = new ApprovePurchaseOrderCommand(orderId, Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*Draft veya Pending*");
    }

    [Fact]
    public async Task Handle_NonExistentOrder_ShouldThrowKeyNotFound()
    {
        var orderId = Guid.NewGuid();
        _repositoryMock.Setup(r => r.GetByIdAsync(orderId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PurchaseOrder?)null);

        var command = new ApprovePurchaseOrderCommand(orderId, Guid.NewGuid());

        var act = () => _handler.Handle(command, CancellationToken.None);

        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
