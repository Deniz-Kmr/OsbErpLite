using FluentAssertions;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Domain.Tests.Entities;

public class OrderStatusTransitionTests
{
    [Fact]
    public void PurchaseOrder_DefaultStatus_ShouldBeDraft()
    {
        var order = new PurchaseOrder();

        order.Status.Should().Be(OrderStatus.Draft);
    }

    [Fact]
    public void SalesOrder_DefaultStatus_ShouldBeDraft()
    {
        var order = new SalesOrder();

        order.Status.Should().Be(OrderStatus.Draft);
    }

    [Theory]
    [InlineData(OrderStatus.Draft, OrderStatus.Approved, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Approved, true)]
    [InlineData(OrderStatus.Draft, OrderStatus.Rejected, true)]
    [InlineData(OrderStatus.Pending, OrderStatus.Rejected, true)]
    [InlineData(OrderStatus.Approved, OrderStatus.Completed, true)]
    [InlineData(OrderStatus.Rejected, OrderStatus.Approved, false)]
    [InlineData(OrderStatus.Completed, OrderStatus.Approved, false)]
    [InlineData(OrderStatus.Completed, OrderStatus.Draft, false)]
    [InlineData(OrderStatus.Rejected, OrderStatus.Completed, false)]
    [InlineData(OrderStatus.Approved, OrderStatus.Draft, false)]
    public void OrderStatus_TransitionRules_ShouldBeRespected(
        OrderStatus from, OrderStatus to, bool isValid)
    {
        var allowed = IsValidTransition(from, to);

        allowed.Should().Be(isValid);
    }

    [Fact]
    public void PurchaseOrder_Lines_ShouldBeInitializedAsEmptyList()
    {
        var order = new PurchaseOrder();

        order.Lines.Should().NotBeNull();
        order.Lines.Should().BeEmpty();
    }

    [Fact]
    public void SalesOrder_Lines_ShouldBeInitializedAsEmptyList()
    {
        var order = new SalesOrder();

        order.Lines.Should().NotBeNull();
        order.Lines.Should().BeEmpty();
    }

    [Fact]
    public void PurchaseOrder_TotalAmount_ShouldBeCalculatedFromLines()
    {
        var order = new PurchaseOrder
        {
            Lines = new List<PurchaseOrderLine>
            {
                new() { Quantity = 5, UnitPrice = 10.00m },
                new() { Quantity = 3, UnitPrice = 20.50m }
            }
        };

        var total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);

        total.Should().Be(111.50m);
    }

    [Fact]
    public void SalesOrder_TotalAmount_ShouldBeCalculatedFromLines()
    {
        var order = new SalesOrder
        {
            Lines = new List<SalesOrderLine>
            {
                new() { Quantity = 2, UnitPrice = 100.00m },
                new() { Quantity = 1, UnitPrice = 250.00m }
            }
        };

        var total = order.Lines.Sum(l => l.Quantity * l.UnitPrice);

        total.Should().Be(450.00m);
    }

    private static bool IsValidTransition(OrderStatus from, OrderStatus to)
    {
        return (from, to) switch
        {
            (OrderStatus.Draft, OrderStatus.Approved) => true,
            (OrderStatus.Draft, OrderStatus.Rejected) => true,
            (OrderStatus.Pending, OrderStatus.Approved) => true,
            (OrderStatus.Pending, OrderStatus.Rejected) => true,
            (OrderStatus.Approved, OrderStatus.Completed) => true,
            _ => false
        };
    }
}
