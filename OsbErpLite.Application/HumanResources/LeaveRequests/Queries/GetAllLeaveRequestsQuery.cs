using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Queries;

public record GetAllLeaveRequestsQuery(
    int Page = 1,
    int PageSize = 10,
    Guid? EmployeeId = null,
    LeaveRequestStatus? Status = null
) : IRequest<PaginatedList<LeaveRequestDto>>;
