using MediatR;
using OsbErpLite.Application.HumanResources.Positions.Dtos;

namespace OsbErpLite.Application.HumanResources.Positions.Queries;

public record GetPositionByIdQuery(Guid Id) : IRequest<PositionDto?>;
