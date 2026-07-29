using OsbErpLite.Domain.Common;

namespace OsbErpLite.Domain.Entities;

public class PurchaseOrderLine : BaseEntity
{
    public Guid PurchaseOrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // navigation
    public PurchaseOrder PurchaseOrder { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
