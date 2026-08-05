using MediatR;

namespace OsbErpLite.Application.Warehouses.Commands;

public record DeleteWarehouseCommand(Guid Id) : IRequest<Unit>;
