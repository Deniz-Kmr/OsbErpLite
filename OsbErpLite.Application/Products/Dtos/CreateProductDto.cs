namespace OsbErpLite.Application.Products.Dtos;

public class CreateProductDto
{
    public string Name { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public Guid WarehouseId { get; set; }
}
