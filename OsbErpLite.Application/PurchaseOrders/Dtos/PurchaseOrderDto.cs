using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.PurchaseOrders.Dtos;

public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = default!;
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public Guid? ApprovedByEmployeeId { get; set; }
    public string? ApprovedByEmployeeName { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public List<PurchaseOrderLineDto> Lines { get; set; } = new();
}
