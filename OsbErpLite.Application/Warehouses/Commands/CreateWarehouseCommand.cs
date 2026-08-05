using MediatR;
using OsbErpLite.Application.Warehouses.Dtos;

namespace OsbErpLite.Application.Warehouses.Commands;

public record CreateWarehouseCommand(
    string Name,
    string? Location
) : IRequest<WarehouseDto>;
