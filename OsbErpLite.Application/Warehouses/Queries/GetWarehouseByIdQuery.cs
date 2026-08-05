using MediatR;
using OsbErpLite.Application.Warehouses.Dtos;

namespace OsbErpLite.Application.Warehouses.Queries;

public record GetWarehouseByIdQuery(Guid Id) : IRequest<WarehouseDto?>;
