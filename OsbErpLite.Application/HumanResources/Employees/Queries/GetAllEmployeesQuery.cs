using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Employees.Dtos;

namespace OsbErpLite.Application.HumanResources.Employees.Queries;

public record GetAllEmployeesQuery(
    int Page = PaginationDefaults.DefaultPage,
    int PageSize = PaginationDefaults.DefaultPageSize,
    Guid? DepartmentId = null,
    Guid? PositionId = null
) : IRequest<PaginatedList<EmployeeDto>>;
