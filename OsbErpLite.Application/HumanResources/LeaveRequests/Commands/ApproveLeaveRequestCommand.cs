using MediatR;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Commands;

public record ApproveLeaveRequestCommand(Guid Id, Guid ApprovedByEmployeeId) : IRequest<LeaveRequestDto>;
