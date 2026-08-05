using MediatR;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Commands;

public record RejectLeaveRequestCommand(Guid Id, Guid RejectedByEmployeeId) : IRequest<LeaveRequestDto>;
