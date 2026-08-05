using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Employees.Dtos;

namespace OsbErpLite.Application.HumanResources.Employees.Queries;

public record GetAllEmployeesQuery(
    int Page = 1,
    int PageSize = 10,
    Guid? DepartmentId = null,
    Guid? PositionId = null
) : IRequest<PaginatedList<EmployeeDto>>;
