using OsbErpLite.Domain.Common;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Domain.Entities;

public class SalesOrder : AuditableEntity
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Draft;
    public Guid? ApprovedByEmployeeId { get; set; }
    public decimal TotalAmount { get; set; }

    // navigation
    public BusinessPartner Customer { get; set; } = default!;
    public Employee? ApprovedByEmployee { get; set; }
    public ICollection<SalesOrderLine> Lines { get; set; } = new List<SalesOrderLine>();
}
