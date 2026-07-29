using OsbErpLite.Domain.Common;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Domain.Entities;

public class PurchaseOrder : AuditableEntity
{
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public Guid? ApprovedByEmployeeId { get; set; }
    public decimal TotalAmount { get; set; }

    // navigation
    public BusinessPartner Supplier { get; set; } = default!;
    public Employee? ApprovedByEmployee { get; set; }
    public ICollection<PurchaseOrderLine> Lines { get; set; } = new List<PurchaseOrderLine>();
}
