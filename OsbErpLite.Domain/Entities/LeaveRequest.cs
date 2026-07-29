using OsbErpLite.Domain.Common;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Domain.Entities;

public class LeaveRequest : AuditableEntity
{
    public Guid EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
    public LeaveRequestStatus Status { get; set; } = LeaveRequestStatus.Pending;
    public Guid? ApprovedByEmployeeId { get; set; }

    // navigation
    public Employee Employee { get; set; } = default!;
    public Employee? ApprovedByEmployee { get; set; }
}
