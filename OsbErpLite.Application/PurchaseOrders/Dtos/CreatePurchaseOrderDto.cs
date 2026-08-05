namespace OsbErpLite.Application.PurchaseOrders.Dtos;

public class CreatePurchaseOrderDto
{
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<CreatePurchaseOrderLineDto> Lines { get; set; } = new();
}

public class CreatePurchaseOrderLineDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
