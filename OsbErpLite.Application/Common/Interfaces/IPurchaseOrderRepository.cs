using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.Common.Interfaces;

public interface IPurchaseOrderRepository
{
    Task<PurchaseOrder?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<PurchaseOrder> Items, int TotalCount)> GetAllAsync(int page, int pageSize, OrderStatus? status = null, CancellationToken cancellationToken = default);
    Task<PurchaseOrder> AddAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(PurchaseOrder entity, CancellationToken cancellationToken = default);
}
