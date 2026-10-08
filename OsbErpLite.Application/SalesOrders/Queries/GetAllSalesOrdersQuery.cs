using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.SalesOrders.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.SalesOrders.Queries;

public record GetAllSalesOrdersQuery(int Page = PaginationDefaults.DefaultPage, int PageSize = PaginationDefaults.DefaultPageSize, OrderStatus? Status = null)
    : IRequest<PaginatedList<SalesOrderDto>>;
