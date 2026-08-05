using MediatR;
using OsbErpLite.Application.StockMovements.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.StockMovements.Commands;

public record CreateStockMovementCommand(
    Guid ProductId,
    int Quantity,
    StockMovementType MovementType,
    string? Note
) : IRequest<StockMovementDto>;
