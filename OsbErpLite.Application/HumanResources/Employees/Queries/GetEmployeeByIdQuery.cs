using MediatR;
using OsbErpLite.Application.HumanResources.Employees.Dtos;

namespace OsbErpLite.Application.HumanResources.Employees.Queries;

public record GetEmployeeByIdQuery(Guid Id) : IRequest<EmployeeDto?>;
