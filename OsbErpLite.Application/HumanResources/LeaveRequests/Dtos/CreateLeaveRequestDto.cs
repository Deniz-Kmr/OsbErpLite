namespace OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;

public class CreateLeaveRequestDto
{
    public Guid EmployeeId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string? Reason { get; set; }
}
