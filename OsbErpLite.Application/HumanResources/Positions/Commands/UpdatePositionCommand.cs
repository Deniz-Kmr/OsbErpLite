using MediatR;
using OsbErpLite.Application.HumanResources.Positions.Dtos;

namespace OsbErpLite.Application.HumanResources.Positions.Commands;

public record UpdatePositionCommand(
    Guid Id,
    string Title,
    int Level
) : IRequest<PositionDto>;
