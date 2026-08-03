using Microsoft.EntityFrameworkCore;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Infrastructure.Persistence.Context;

namespace OsbErpLite.Infrastructure.Persistence.Repositories;

public class BusinessPartnerRepository : IBusinessPartnerRepository
{
    private readonly AppDbContext _context;

    public BusinessPartnerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BusinessPartner?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.BusinessPartners.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<(List<BusinessPartner> Items, int TotalCount)> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.BusinessPartners.Where(bp => bp.IsActive).AsNoTracking();

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(bp => bp.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<BusinessPartner> AddAsync(BusinessPartner entity, CancellationToken cancellationToken = default)
    {
        await _context.BusinessPartners.AddAsync(entity, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(BusinessPartner entity, CancellationToken cancellationToken = default)
    {
        _context.BusinessPartners.Update(entity);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
