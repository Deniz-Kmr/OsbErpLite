using MediatR;
using Microsoft.Extensions.Logging;
using OsbErpLite.Application.BusinessPartners.Dtos;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.BusinessPartners.Commands;

public class SyncBusinessPartnersFromSapCommandHandler
    : IRequestHandler<SyncBusinessPartnersFromSapCommand, SyncResultDto>
{
    private readonly ISapBusinessPartnerClient _sapClient;
    private readonly IBusinessPartnerRepository _repository;
    private readonly ILogger<SyncBusinessPartnersFromSapCommandHandler> _logger;

    public SyncBusinessPartnersFromSapCommandHandler(
        ISapBusinessPartnerClient sapClient,
        IBusinessPartnerRepository repository,
        ILogger<SyncBusinessPartnersFromSapCommandHandler> logger)
    {
        _sapClient = sapClient;
        _repository = repository;
        _logger = logger;
    }

    public async Task<SyncResultDto> Handle(
        SyncBusinessPartnersFromSapCommand request,
        CancellationToken cancellationToken)
    {
        var result = new SyncResultDto();

        var sapRecords = await _sapClient.GetBusinessPartnersAsync(cancellationToken);
        _logger.LogInformation("SAP'den {Count} kayit alindi, eslestirme basliyor.", sapRecords.Count);

        foreach (var record in sapRecords)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(record.TaxNumber))
                {
                    result.Errors.Add($"SAP ID '{record.SapId}': TaxNumber bos, kayit atlandi.");
                    continue;
                }

                var existing = await _repository.GetByTaxNumberAsync(record.TaxNumber, cancellationToken);

                if (existing is not null)
                {
                    existing.Name = record.Name ?? record.SapId ?? existing.Name;
                    existing.Type = MapCategory(record.Category);
                    existing.UpdatedDate = DateTime.UtcNow;
                    await _repository.UpdateAsync(existing, cancellationToken);
                    result.UpdatedCount++;
                }
                else
                {
                    var entity = new BusinessPartner
                    {
                        Id = Guid.NewGuid(),
                        Name = record.Name ?? record.SapId ?? "Unknown",
                        TaxNumber = record.TaxNumber,
                        Type = MapCategory(record.Category),
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };
                    await _repository.AddAsync(entity, cancellationToken);
                    result.AddedCount++;
                }
            }
            catch (Exception ex)
            {
                result.Errors.Add($"SAP ID '{record.SapId}': {ex.Message}");
                _logger.LogWarning(ex, "SAP kaydi islenirken hata. SapId: {SapId}", record.SapId);
            }
        }

        _logger.LogInformation(
            "SAP sync tamamlandi. Eklenen: {Added}, Guncellenen: {Updated}, Hata: {ErrorCount}",
            result.AddedCount, result.UpdatedCount, result.Errors.Count);

        return result;
    }

    private static BusinessPartnerType MapCategory(string? category)
    {
        return category switch
        {
            "1" => BusinessPartnerType.Supplier,
            "2" => BusinessPartnerType.Customer,
            _ => BusinessPartnerType.Customer
        };
    }
}
