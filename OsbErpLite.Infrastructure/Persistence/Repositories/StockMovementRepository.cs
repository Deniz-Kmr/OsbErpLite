using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly AppDbContext _context;

    public StockMovementRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<StockMovement> AddAsync(StockMovement entity, CancellationToken cancellationToken = default)
    {
        await _context.StockMovements.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity).Reference(sm => sm.Product).LoadAsync(cancellationToken);
        return entity;
    }

    public async Task<(List<StockMovement> Items, int TotalCount)> GetByProductIdAsync(Guid productId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.StockMovements
            .Include(sm => sm.Product)
            .Where(sm => sm.ProductId == productId)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(sm => sm.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
