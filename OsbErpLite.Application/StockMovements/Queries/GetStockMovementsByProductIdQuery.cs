using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.StockMovements.Dtos;

namespace OsbErpLite.Application.StockMovements.Queries;

public record GetStockMovementsByProductIdQuery(Guid ProductId, int Page = 1, int PageSize = 10) : IRequest<PaginatedList<StockMovementDto>>;
