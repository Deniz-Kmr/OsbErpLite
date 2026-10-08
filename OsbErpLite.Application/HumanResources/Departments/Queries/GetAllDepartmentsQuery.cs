using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Departments.Dtos;

namespace OsbErpLite.Application.HumanResources.Departments.Queries;

public record GetAllDepartmentsQuery(int Page = PaginationDefaults.DefaultPage, int PageSize = PaginationDefaults.DefaultPageSize)
    : IRequest<PaginatedList<DepartmentDto>>;
