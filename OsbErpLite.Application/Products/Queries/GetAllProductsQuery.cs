using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.Products.Dtos;

namespace OsbErpLite.Application.Products.Queries;

public record GetAllProductsQuery(int Page = PaginationDefaults.DefaultPage, int PageSize = PaginationDefaults.DefaultPageSize, Guid? WarehouseId = null) : IRequest<PaginatedList<ProductDto>>;
