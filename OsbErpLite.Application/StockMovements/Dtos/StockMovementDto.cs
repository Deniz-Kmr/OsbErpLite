using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.StockMovements.Dtos;

public class StockMovementDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = default!;
    public int Quantity { get; set; }
    public StockMovementType MovementType { get; set; }
    public DateTime Date { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedDate { get; set; }
}
