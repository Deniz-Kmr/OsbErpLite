using MediatR;
using Microsoft.Extensions.Options;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Dashboard.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Dashboard.Queries;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IDashboardRepository _repository;
    private readonly DashboardSettings _settings;

    public GetDashboardSummaryQueryHandler(
        IDashboardRepository repository,
        IOptions<DashboardSettings> settings)
    {
        _repository = repository;
        _settings = settings.Value;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var employeeCount = await _repository.GetActiveEmployeeCountAsync(cancellationToken);
        var partnerCounts = await _repository.GetActiveBusinessPartnerCountsByTypeAsync(cancellationToken);
        var pendingPurchase = await _repository.GetPendingPurchaseOrdersCountAsync(cancellationToken);
        var pendingSales = await _repository.GetPendingSalesOrdersCountAsync(cancellationToken);
        var pendingLeave = await _repository.GetPendingLeaveRequestsCountAsync(cancellationToken);
        var lowStock = await _repository.GetLowStockProductsCountAsync(_settings.LowStockThreshold, cancellationToken);

        var dto = new DashboardSummaryDto
        {
            TotalActiveEmployeeCount = employeeCount,
            ActiveCustomerCount = partnerCounts.GetValueOrDefault(BusinessPartnerType.Customer),
            ActiveSupplierCount = partnerCounts.GetValueOrDefault(BusinessPartnerType.Supplier),
            ActiveBothCount = partnerCounts.GetValueOrDefault(BusinessPartnerType.Both),
            PendingPurchaseOrdersCount = pendingPurchase,
            PendingSalesOrdersCount = pendingSales,
            PendingLeaveRequestsCount = pendingLeave,
            LowStockProductsCount = lowStock
        };

        if (request.IncludeFinancialData)
        {
            dto.TotalStockValue = await _repository.GetTotalStockValueAsync(cancellationToken);
        }

        return dto;
    }
}
