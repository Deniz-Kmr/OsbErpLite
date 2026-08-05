using MediatR;

namespace OsbErpLite.Application.HumanResources.Positions.Commands;

public record DeletePositionCommand(Guid Id) : IRequest<Unit>;
