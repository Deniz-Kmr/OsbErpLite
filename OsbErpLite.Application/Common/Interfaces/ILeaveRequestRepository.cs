using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Common.Interfaces;

public interface ILeaveRequestRepository
{
    Task<LeaveRequest?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<LeaveRequest> Items, int TotalCount)> GetAllAsync(int page, int pageSize, Guid? employeeId = null, LeaveRequestStatus? status = null, CancellationToken cancellationToken = default);
    Task<LeaveRequest> AddAsync(LeaveRequest entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(LeaveRequest entity, CancellationToken cancellationToken = default);
}
