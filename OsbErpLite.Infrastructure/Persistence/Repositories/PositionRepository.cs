using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class PositionRepository : IPositionRepository
{
    private readonly AppDbContext _context;

    public PositionRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Position?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Positions.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<(List<Position> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Positions
            .Where(p => p.IsActive)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Position> AddAsync(Position entity, CancellationToken cancellationToken = default)
    {
        await _context.Positions.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Position entity, CancellationToken cancellationToken = default)
    {
        _context.Positions.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
