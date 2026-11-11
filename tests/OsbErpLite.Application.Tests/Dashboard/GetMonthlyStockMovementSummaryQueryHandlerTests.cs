using FluentAssertions;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Dashboard.Dtos;
using OsbErpLite.Application.Dashboard.Queries;

namespace OsbErpLite.Application.Tests.Dashboard;

public class GetMonthlyStockMovementSummaryQueryHandlerTests
{
    private readonly Mock<IDashboardRepository> _repositoryMock;
    private readonly GetMonthlyStockMovementSummaryQueryHandler _handler;

    public GetMonthlyStockMovementSummaryQueryHandlerTests()
    {
        _repositoryMock = new Mock<IDashboardRepository>();
        _handler = new GetMonthlyStockMovementSummaryQueryHandler(_repositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnSixMonths()
    {
        _repositoryMock.Setup(r => r.GetMonthlyStockMovementsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MonthlyStockMovementDto>());

        var result = await _handler.Handle(new GetMonthlyStockMovementSummaryQuery(), CancellationToken.None);

        result.Should().HaveCount(6);
    }

    [Fact]
    public async Task Handle_EmptyMonths_ShouldFillWithZeros()
    {
        _repositoryMock.Setup(r => r.GetMonthlyStockMovementsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MonthlyStockMovementDto>());

        var result = await _handler.Handle(new GetMonthlyStockMovementSummaryQuery(), CancellationToken.None);

        result.Should().AllSatisfy(m =>
        {
            m.TotalIn.Should().Be(0);
            m.TotalOut.Should().Be(0);
        });
    }

    [Fact]
    public async Task Handle_WithData_ShouldMergeCorrectly()
    {
        var now = DateTime.UtcNow;
        var dbData = new List<MonthlyStockMovementDto>
        {
            new() { Year = now.Year, Month = now.Month, TotalIn = 100, TotalOut = 50 }
        };

        _repositoryMock.Setup(r => r.GetMonthlyStockMovementsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(dbData);

        var result = await _handler.Handle(new GetMonthlyStockMovementSummaryQuery(), CancellationToken.None);

        var currentMonth = result.Last();
        currentMonth.TotalIn.Should().Be(100);
        currentMonth.TotalOut.Should().Be(50);
    }

    [Fact]
    public async Task Handle_MonthsInChronologicalOrder()
    {
        _repositoryMock.Setup(r => r.GetMonthlyStockMovementsAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<MonthlyStockMovementDto>());

        var result = await _handler.Handle(new GetMonthlyStockMovementSummaryQuery(), CancellationToken.None);

        for (int i = 1; i < result.Count; i++)
        {
            var prev = new DateTime(result[i - 1].Year, result[i - 1].Month, 1);
            var curr = new DateTime(result[i].Year, result[i].Month, 1);
            curr.Should().BeAfter(prev);
        }
    }
}
