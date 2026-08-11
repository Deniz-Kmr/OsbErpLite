using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Common.Interfaces;

public interface IBusinessPartnerRepository
{
    Task<BusinessPartner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<(List<BusinessPartner> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<BusinessPartner> AddAsync(BusinessPartner entity, CancellationToken cancellationToken = default);
    Task UpdateAsync(BusinessPartner entity, CancellationToken cancellationToken = default);
    Task<BusinessPartner?> GetByTaxNumberAsync(string taxNumber, CancellationToken cancellationToken = default);
    Task<List<BusinessPartner>> GetAllActiveAsync(CancellationToken cancellationToken = default);
}
