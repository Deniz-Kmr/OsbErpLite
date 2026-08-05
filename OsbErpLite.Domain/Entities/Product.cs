using OsbErpLite.Domain.Common;

namespace OsbErpLite.Domain.Entities;

public class Product : AuditableEntity
{
    public string Name { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid WarehouseId { get; set; }

    // navigation
    public Warehouse Warehouse { get; set; } = default!;
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
    public ICollection<PurchaseOrderLine> PurchaseOrderLines { get; set; } = new List<PurchaseOrderLine>();
    public ICollection<SalesOrderLine> SalesOrderLines { get; set; } = new List<SalesOrderLine>();
}
