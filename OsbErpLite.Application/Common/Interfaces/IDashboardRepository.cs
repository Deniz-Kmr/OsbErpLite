using OsbErpLite.Application.Dashboard.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Common.Interfaces;

public interface IDashboardRepository
{
    Task<int> GetActiveEmployeeCountAsync(CancellationToken cancellationToken = default);
    Task<Dictionary<BusinessPartnerType, int>> GetActiveBusinessPartnerCountsByTypeAsync(CancellationToken cancellationToken = default);
    Task<int> GetPendingPurchaseOrdersCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetPendingSalesOrdersCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetPendingLeaveRequestsCountAsync(CancellationToken cancellationToken = default);
    Task<int> GetLowStockProductsCountAsync(int threshold, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default);
    Task<List<MonthlyStockMovementDto>> GetMonthlyStockMovementsAsync(DateTime startDate, CancellationToken cancellationToken = default);
}
