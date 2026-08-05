using MediatR;
using OsbErpLite.Application.HumanResources.Departments.Dtos;

namespace OsbErpLite.Application.HumanResources.Departments.Queries;

public record GetDepartmentByIdQuery(Guid Id) : IRequest<DepartmentDto?>;
