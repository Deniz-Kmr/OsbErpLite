using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.StockMovements.Dtos;

namespace OsbErpLite.Application.StockMovements.Queries;

public record GetStockMovementsByProductIdQuery(Guid ProductId, int Page = PaginationDefaults.DefaultPage, int PageSize = PaginationDefaults.DefaultPageSize) : IRequest<PaginatedList<StockMovementDto>>;
