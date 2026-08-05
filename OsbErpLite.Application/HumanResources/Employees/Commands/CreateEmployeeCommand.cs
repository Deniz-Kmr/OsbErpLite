using MediatR;
using OsbErpLite.Application.HumanResources.Employees.Dtos;

namespace OsbErpLite.Application.HumanResources.Employees.Commands;

public record CreateEmployeeCommand(
    string FullName,
    string NationalId,
    Guid DepartmentId,
    Guid PositionId,
    Guid? ManagerId,
    DateTime HireDate,
    decimal Salary
) : IRequest<EmployeeDto>;
