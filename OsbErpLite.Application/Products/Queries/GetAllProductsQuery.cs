using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Products.Dtos;

namespace OsbErpLite.Application.Products.Queries;

public record GetAllProductsQuery(int Page = 1, int PageSize = 10, Guid? WarehouseId = null) : IRequest<PaginatedList<ProductDto>>;
