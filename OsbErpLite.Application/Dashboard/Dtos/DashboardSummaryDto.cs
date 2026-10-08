namespace OsbErpLite.Application.Dashboard.Dtos;

public class DashboardSummaryDto
{
    public int TotalActiveEmployeeCount { get; set; }
    public int ActiveCustomerCount { get; set; }
    public int ActiveSupplierCount { get; set; }
    public int ActiveBothCount { get; set; }
    public int PendingPurchaseOrdersCount { get; set; }
    public int PendingSalesOrdersCount { get; set; }
    public int PendingLeaveRequestsCount { get; set; }
    public int LowStockProductsCount { get; set; }
    public decimal? TotalStockValue { get; set; }
}
