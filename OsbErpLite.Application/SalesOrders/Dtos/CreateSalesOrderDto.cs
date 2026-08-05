namespace OsbErpLite.Application.SalesOrders.Dtos;

public class CreateSalesOrderDto
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<CreateSalesOrderLineDto> Lines { get; set; } = new();
}

public class CreateSalesOrderLineDto
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
