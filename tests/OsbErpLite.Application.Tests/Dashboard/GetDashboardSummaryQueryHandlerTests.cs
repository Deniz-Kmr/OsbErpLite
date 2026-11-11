using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Dashboard.Dtos;
using OsbErpLite.Application.Dashboard.Queries;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Tests.Dashboard;

public class GetDashboardSummaryQueryHandlerTests
{
    private readonly Mock<IDashboardRepository> _repositoryMock;
    private readonly IOptions<DashboardSettings> _settings;
    private readonly GetDashboardSummaryQueryHandler _handler;

    public GetDashboardSummaryQueryHandlerTests()
    {
        _repositoryMock = new Mock<IDashboardRepository>();
        _settings = Options.Create(new DashboardSettings { LowStockThreshold = 10 });
        _handler = new GetDashboardSummaryQueryHandler(_repositoryMock.Object, _settings);
    }

    [Fact]
    public async Task Handle_IncludeFinancialTrue_ShouldReturnTotalStockValue()
    {
        SetupDefaultMocks();
        _repositoryMock.Setup(r => r.GetTotalStockValueAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(50000m);

        var result = await _handler.Handle(new GetDashboardSummaryQuery(true), CancellationToken.None);

        result.TotalStockValue.Should().Be(50000m);
        _repositoryMock.Verify(r => r.GetTotalStockValueAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_IncludeFinancialFalse_ShouldReturnNullTotalStockValue()
    {
        SetupDefaultMocks();

        var result = await _handler.Handle(new GetDashboardSummaryQuery(false), CancellationToken.None);

        result.TotalStockValue.Should().BeNull();
        _repositoryMock.Verify(r => r.GetTotalStockValueAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ShouldUseLowStockThresholdFromSettings()
    {
        SetupDefaultMocks();
        _repositoryMock.Setup(r => r.GetLowStockProductsCountAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var result = await _handler.Handle(new GetDashboardSummaryQuery(false), CancellationToken.None);

        result.LowStockProductsCount.Should().Be(3);
        _repositoryMock.Verify(r => r.GetLowStockProductsCountAsync(10, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CustomThreshold_ShouldPassToRepository()
    {
        var customSettings = Options.Create(new DashboardSettings { LowStockThreshold = 25 });
        var handler = new GetDashboardSummaryQueryHandler(_repositoryMock.Object, customSettings);
        SetupDefaultMocks();

        await handler.Handle(new GetDashboardSummaryQuery(false), CancellationToken.None);

        _repositoryMock.Verify(r => r.GetLowStockProductsCountAsync(25, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldReturnCorrectBusinessPartnerCounts()
    {
        SetupDefaultMocks();
        _repositoryMock.Setup(r => r.GetActiveBusinessPartnerCountsByTypeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<BusinessPartnerType, int>
            {
                { BusinessPartnerType.Customer, 5 },
                { BusinessPartnerType.Supplier, 3 },
                { BusinessPartnerType.Both, 2 }
            });

        var result = await _handler.Handle(new GetDashboardSummaryQuery(false), CancellationToken.None);

        result.ActiveCustomerCount.Should().Be(5);
        result.ActiveSupplierCount.Should().Be(3);
        result.ActiveBothCount.Should().Be(2);
    }

    [Fact]
    public async Task Handle_NoBusinessPartners_ShouldReturnZeroCounts()
    {
        SetupDefaultMocks();
        _repositoryMock.Setup(r => r.GetActiveBusinessPartnerCountsByTypeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<BusinessPartnerType, int>());

        var result = await _handler.Handle(new GetDashboardSummaryQuery(false), CancellationToken.None);

        result.ActiveCustomerCount.Should().Be(0);
        result.ActiveSupplierCount.Should().Be(0);
        result.ActiveBothCount.Should().Be(0);
    }

    private void SetupDefaultMocks()
    {
        _repositoryMock.Setup(r => r.GetActiveEmployeeCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(10);
        _repositoryMock.Setup(r => r.GetActiveBusinessPartnerCountsByTypeAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Dictionary<BusinessPartnerType, int>());
        _repositoryMock.Setup(r => r.GetPendingPurchaseOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _repositoryMock.Setup(r => r.GetPendingSalesOrdersCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _repositoryMock.Setup(r => r.GetPendingLeaveRequestsCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        _repositoryMock.Setup(r => r.GetLowStockProductsCountAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);
    }
}
