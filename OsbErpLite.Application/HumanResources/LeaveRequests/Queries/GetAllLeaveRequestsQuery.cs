using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Queries;

public record GetAllLeaveRequestsQuery(
    int Page = PaginationDefaults.DefaultPage,
    int PageSize = PaginationDefaults.DefaultPageSize,
    Guid? EmployeeId = null,
    LeaveRequestStatus? Status = null
) : IRequest<PaginatedList<LeaveRequestDto>>;
