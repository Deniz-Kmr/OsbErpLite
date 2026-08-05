namespace OsbErpLite.Application.Products.Dtos;

public class UpdateProductDto
{
    public string Name { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public Guid WarehouseId { get; set; }
}
