using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Common.Interfaces;

public interface IStockMovementRepository
{
    Task<StockMovement> AddAsync(StockMovement entity, CancellationToken cancellationToken = default);
    Task<(List<StockMovement> Items, int TotalCount)> GetByProductIdAsync(Guid productId, int page, int pageSize, CancellationToken cancellationToken = default);
}
