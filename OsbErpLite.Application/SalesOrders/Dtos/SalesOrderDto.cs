using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.SalesOrders.Dtos;

public class SalesOrderDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public string StatusName { get; set; } = default!;
    public Guid? ApprovedByEmployeeId { get; set; }
    public string? ApprovedByEmployeeName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public List<SalesOrderLineDto> Lines { get; set; } = new();
}
