using MediatR;
using OsbErpLite.Application.Products.Dtos;

namespace OsbErpLite.Application.Products.Commands;

public record CreateProductCommand(
    string Name,
    string SKU,
    decimal UnitPrice,
    int StockQuantity,
    Guid WarehouseId
) : IRequest<ProductDto>;
