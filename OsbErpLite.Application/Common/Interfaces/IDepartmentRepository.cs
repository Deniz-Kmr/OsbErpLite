using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Common.Interfaces;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Department> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Department> AddAsync(Department entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Department entity, CancellationToken cancellationToken = default);
    Task<bool> IsManagerOfAnyDepartmentAsync(Guid employeeId, CancellationToken cancellationToken = default);
}
