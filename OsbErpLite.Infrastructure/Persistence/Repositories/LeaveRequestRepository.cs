using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class LeaveRequestRepository : ILeaveRequestRepository
{
    private readonly AppDbContext _context;

    public LeaveRequestRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.ApprovedByEmployee)
            .FirstOrDefaultAsync(lr => lr.Id == id, cancellationToken);
    }

    public async Task<(List<LeaveRequest> Items, int TotalCount)> GetAllAsync(
        int page, int pageSize, Guid? employeeId = null, LeaveRequestStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.LeaveRequests
            .Include(lr => lr.Employee)
            .Include(lr => lr.ApprovedByEmployee)
            .AsNoTracking()
            .AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(lr => lr.EmployeeId == employeeId.Value);

        if (status.HasValue)
            query = query.Where(lr => lr.Status == status.Value);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(lr => lr.CreatedDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<LeaveRequest> AddAsync(LeaveRequest entity, CancellationToken cancellationToken = default)
    {
        await _context.LeaveRequests.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(entity).Reference(lr => lr.Employee).LoadAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(LeaveRequest entity, CancellationToken cancellationToken = default)
    {
        _context.LeaveRequests.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountPendingByEmployeeIdAsync(Guid employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.LeaveRequests
            .CountAsync(lr => lr.EmployeeId == employeeId && lr.Status == LeaveRequestStatus.Pending, cancellationToken);
    }
}
