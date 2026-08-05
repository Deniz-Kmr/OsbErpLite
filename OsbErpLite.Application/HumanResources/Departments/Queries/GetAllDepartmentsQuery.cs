using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Departments.Dtos;

namespace OsbErpLite.Application.HumanResources.Departments.Queries;

public record GetAllDepartmentsQuery(int Page = 1, int PageSize = 10)
    : IRequest<PaginatedList<DepartmentDto>>;
