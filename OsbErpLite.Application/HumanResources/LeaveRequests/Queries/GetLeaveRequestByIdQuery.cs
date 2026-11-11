using MediatR;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Queries;

public record GetLeaveRequestByIdQuery(Guid Id) : IRequest<LeaveRequestDto?>;
