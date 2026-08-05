using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Common.Interfaces;

public interface ISalesOrderRepository
{
    Task<SalesOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<SalesOrder> Items, int TotalCount)> GetAllAsync(int page, int pageSize, OrderStatus? status = null, CancellationToken cancellationToken = default);
    Task<SalesOrder> AddAsync(SalesOrder entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(SalesOrder entity, CancellationToken cancellationToken = default);
}
