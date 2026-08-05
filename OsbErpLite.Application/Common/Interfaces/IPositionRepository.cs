using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Common.Interfaces;

public interface IPositionRepository
{
    Task<Position?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<Position> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Position> AddAsync(Position entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(Position entity, CancellationToken cancellationToken = default);
}
