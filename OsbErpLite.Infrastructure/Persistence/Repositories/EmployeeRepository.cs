using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<(List<Employee> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, Guid? departmentId = null, Guid? positionId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Employees
            .Where(e => e.IsActive)
            .Include(e => e.Department)
            .Include(e => e.Position)
            .Include(e => e.Manager)
            .AsNoTracking()
            .AsQueryable();

        if (departmentId.HasValue)
            query = query.Where(e => e.DepartmentId == departmentId.Value);

        if (positionId.HasValue)
            query = query.Where(e => e.PositionId == positionId.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(e => e.FullName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<Employee> AddAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        await _context.Employees.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity).Reference(e => e.Department).LoadAsync(cancellationToken);
        await _context.Entry(entity).Reference(e => e.Position).LoadAsync(cancellationToken);
        await _context.Entry(entity).Reference(e => e.Manager).LoadAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Employee entity, CancellationToken cancellationToken = default)
    {
        _context.Employees.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountActiveByDepartmentIdAsync(Guid departmentId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .CountAsync(e => e.DepartmentId == departmentId && e.IsActive && e.Status == Domain.Enums.EmployeeStatus.Active, cancellationToken);
    }

    public async Task<int> CountActiveByPositionIdAsync(Guid positionId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .CountAsync(e => e.PositionId == positionId && e.IsActive && e.Status == Domain.Enums.EmployeeStatus.Active, cancellationToken);
    }

    public async Task<int> CountActiveByManagerIdAsync(Guid managerId, CancellationToken cancellationToken = default)
    {
        return await _context.Employees
            .CountAsync(e => e.ManagerId == managerId && e.IsActive && e.Status == Domain.Enums.EmployeeStatus.Active, cancellationToken);
    }
}
