using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class SalesOrderRepository : ISalesOrderRepository
{
    private readonly AppDbContext _context;

    public SalesOrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SalesOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.ApprovedByEmployee)
            .Include(so => so.Lines)
                .ThenInclude(l => l.Product)
            .FirstOrDefaultAsync(so => so.Id == id, cancellationToken);
    }

    public async Task<(List<SalesOrder> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, OrderStatus? status = null, CancellationToken cancellationToken = default)
    {
        var query = _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.ApprovedByEmployee)
            .Include(so => so.Lines)
                .ThenInclude(l => l.Product)
            .AsNoTracking();

        if (status.HasValue)
            query = query.Where(so => so.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(so => so.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<SalesOrder> AddAsync(SalesOrder entity, CancellationToken cancellationToken = default)
    {
        await _context.SalesOrders.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity).Reference(so => so.Customer).LoadAsync(cancellationToken);
        await _context.Entry(entity).Collection(so => so.Lines).LoadAsync(cancellationToken);
        foreach (var line in entity.Lines)
            await _context.Entry(line).Reference(l => l.Product).LoadAsync(cancellationToken);

        return entity;
    }

    public async Task UpdateAsync(SalesOrder entity, CancellationToken cancellationToken = default)
    {
        _context.SalesOrders.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
