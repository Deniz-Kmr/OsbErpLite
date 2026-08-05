using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.SalesOrders.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.SalesOrders.Queries;

public record GetAllSalesOrdersQuery(int Page = 1, int PageSize = 10, OrderStatus? Status = null)
    : IRequest<PaginatedList<SalesOrderDto>>;
