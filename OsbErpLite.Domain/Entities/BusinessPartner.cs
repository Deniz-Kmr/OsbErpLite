using OsbErpLite.Domain.Common;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Domain.Entities;

public class BusinessPartner : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string? TaxNumber { get; set; }
    public BusinessPartnerType Type { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public bool IsActive { get; set; } = true;

    // navigation
    public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
}
