using MediatR;
using OsbErpLite.Application.Warehouses.Dtos;

namespace OsbErpLite.Application.Warehouses.Commands;

public record UpdateWarehouseCommand(
    Guid Id,
    string Name,
    string? Location
) : IRequest<WarehouseDto>;
