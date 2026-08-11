using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Infrastructure.ExternalServices.Sap;

public static class SapBusinessPartnerMapper
{
    public static BusinessPartner MapToEntity(SapBusinessPartnerRecord record)
    {
        return new BusinessPartner
        {
            Id = Guid.NewGuid(),
            Name = record.Name ?? record.SapId ?? "Unknown",
            TaxNumber = record.TaxNumber,
            Type = MapCategory(record.Category),
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };
    }

    public static void UpdateEntity(BusinessPartner entity, SapBusinessPartnerRecord record)
    {
        entity.Name = record.Name ?? record.SapId ?? entity.Name;
        entity.Type = MapCategory(record.Category);
        entity.UpdatedDate = DateTime.UtcNow;
    }

    private static BusinessPartnerType MapCategory(string? category)
    {
        // SAP BusinessPartnerCategory: 1 = Organization, 2 = Person
        // bizim sistemde net bir eslestirme yok varsayilan olarak Customer atiyorum
        // gercek entegrasyonda SAP rolelerine gore (BP_CustSupp) belirlenebilir
        return category switch
        {
            "1" => BusinessPartnerType.Supplier,
            "2" => BusinessPartnerType.Customer,
            _ => BusinessPartnerType.Customer
        };
    }
}
