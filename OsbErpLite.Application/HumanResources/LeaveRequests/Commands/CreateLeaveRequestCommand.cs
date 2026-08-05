using MediatR;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Commands;

public record CreateLeaveRequestCommand(
    Guid EmployeeId,
    DateTime StartDate,
    DateTime EndDate,
    string? Reason
) : IRequest<LeaveRequestDto>;
