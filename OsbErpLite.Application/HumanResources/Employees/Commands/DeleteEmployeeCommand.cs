using MediatR;

namespace OsbErpLite.Application.HumanResources.Employees.Commands;

public record DeleteEmployeeCommand(Guid Id) : IRequest<Unit>;
