using MediatR;
using OsbErpLite.Application.HumanResources.Departments.Dtos;

namespace OsbErpLite.Application.HumanResources.Departments.Commands;

public record CreateDepartmentCommand(
    string Name,
    Guid? ManagerId
) : IRequest<DepartmentDto>;
