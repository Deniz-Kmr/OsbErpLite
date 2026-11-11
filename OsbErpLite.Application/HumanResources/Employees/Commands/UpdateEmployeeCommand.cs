using MediatR;
using OsbErpLite.Application.HumanResources.Employees.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.HumanResources.Employees.Commands;

public record UpdateEmployeeCommand(
    Guid Id,
    string FullName,
    string NationalId,
    Guid DepartmentId,
    Guid PositionId,
    Guid? ManagerId,
    DateTime HireDate,
    decimal Salary,
    EmployeeStatus Status
) : IRequest<EmployeeDto>;
