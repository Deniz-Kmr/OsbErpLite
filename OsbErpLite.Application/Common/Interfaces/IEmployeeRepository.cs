using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Common.Interfaces;

public interface IEmployeeRepository
{
    Task<Employee?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Employee> Items, int TotalCount)> GetAllAsync(int page, int pageSize, Guid? departmentId = null, Guid? positionId = null, CancellationToken cancellationToken = default);
    Task<Employee> AddAsync(Employee entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Employee entity, CancellationToken cancellationToken = default);
}
