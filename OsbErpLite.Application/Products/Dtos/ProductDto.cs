namespace OsbErpLite.Application.Products.Dtos;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string SKU { get; set; } = default!;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = default!;
    public DateTime CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}
