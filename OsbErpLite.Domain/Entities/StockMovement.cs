using OsbErpLite.Domain.Common;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Domain.Entities;

public class StockMovement : AuditableEntity
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public StockMovementType MovementType { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }

    // navigation
    public Product Product { get; set; } = default!;
}
