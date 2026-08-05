using MediatR;
using OsbErpLite.Application.Products.Dtos;

namespace OsbErpLite.Application.Products.Commands;

public record UpdateProductCommand(
    Guid Id,
    string Name,
    string SKU,
    decimal UnitPrice,
    Guid WarehouseId
) : IRequest<ProductDto>;
