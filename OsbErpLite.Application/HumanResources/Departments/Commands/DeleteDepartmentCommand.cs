using MediatR;

namespace OsbErpLite.Application.HumanResources.Departments.Commands;

public record DeleteDepartmentCommand(Guid Id) : IRequest<Unit>;
