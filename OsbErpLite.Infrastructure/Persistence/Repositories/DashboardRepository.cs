using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Enums;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> GetActiveEmployeeCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .AsNoTracking()
            .CountAsync(e => e.Status == EmployeeStatus.Active, cancellationToken);
    }

    public async Task<Dictionary<BusinessPartnerType, int>> GetActiveBusinessPartnerCountsByTypeAsync(CancellationToken cancellationToken = default)
    {
        return await _context.BusinessPartners
            .AsNoTracking()
            .Where(bp => bp.IsActive)
            .GroupBy(bp => bp.Type)
            .Select(g => new { Type = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Type, x => x.Count, cancellationToken);
    }

    public async Task<int> GetPendingPurchaseOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .AsNoTracking()
            .CountAsync(po => po.Status == OrderStatus.Draft
                           || po.Status == OrderStatus.Pending
                           || po.Status == OrderStatus.Approved, cancellationToken);
    }

    public async Task<int> GetPendingSalesOrdersCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SalesOrders
            .AsNoTracking()
            .CountAsync(so => so.Status == OrderStatus.Draft
                           || so.Status == OrderStatus.Pending
                           || so.Status == OrderStatus.Approved, cancellationToken);
    }

    public async Task<int> GetPendingLeaveRequestsCountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .AsNoTracking()
            .CountAsync(lr => lr.Status == LeaveRequestStatus.Pending, cancellationToken);
    }

    public async Task<int> GetLowStockProductsCountAsync(int threshold, CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .CountAsync(p => p.IsActive && p.StockQuantity < threshold, cancellationToken);
    }

    public async Task<decimal> GetTotalStockValueAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products
            .AsNoTracking()
            .Where(p => p.IsActive)
            .SumAsync(p => p.UnitPrice * p.StockQuantity, cancellationToken);
    }
}
