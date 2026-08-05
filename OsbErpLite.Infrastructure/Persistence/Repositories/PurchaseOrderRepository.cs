using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class PurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly AppDbContext _context;

    public PurchaseOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.ApprovedByEmployee)
            .Include(po => po.Lines)
                .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(po => po.Id == id, cancellationToken);
    }

    public async Task<(List<PurchaseOrder> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, OrderStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.ApprovedByEmployee)
            .Include(po => po.Lines)
                .ThenInclude(l => l.Product)
            .AsNoTracking();

        if (status.HasValue)
            query = query.Where(po => po.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(po => po.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<PurchaseOrder> AddAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
    {
        await _context.PurchaseOrders.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity).Reference(po => po.Supplier).LoadAsync(cancellationToken);
        await _context.Entry(entity).Collection(po => po.Lines).LoadAsync(cancellationToken);
        foreach (var line in entity.Lines)
            await _context.Entry(line).Reference(l => l.Product).LoadAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(PurchaseOrder entity, CancellationToken cancellationToken = default)
    {
        _context.PurchaseOrders.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
