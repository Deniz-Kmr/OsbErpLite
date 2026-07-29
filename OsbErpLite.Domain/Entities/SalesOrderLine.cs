using OsbErpLite.Domain.Common;

namespace OsbErpLite.Domain.Entities;

public class SalesOrderLine : BaseEntity
{
    public Guid SalesOrderId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    // navigation
    public SalesOrder SalesOrder { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
