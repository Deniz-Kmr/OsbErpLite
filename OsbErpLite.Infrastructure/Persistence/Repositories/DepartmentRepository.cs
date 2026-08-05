using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Departments
            .Include(d => d.Manager)
            .FirstOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<(List<Department> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Departments
            .Where(d => d.IsActive)
            .Include(d => d.Manager)
            .AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(d => d.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Department> AddAsync(Department entity, CancellationToken cancellationToken = default)
    {
        await _context.Departments.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity).Reference(d => d.Manager).LoadAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Department entity, CancellationToken cancellationToken = default)
    {
        _context.Departments.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
