using MediatR;
using OsbErpLite.Application.Common;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.PurchaseOrders.Queries;

public record GetAllPurchaseOrdersQuery(int Page = PaginationDefaults.DefaultPage, int PageSize = PaginationDefaults.DefaultPageSize, OrderStatus? Status = null)
    : IRequest<PaginatedList<PurchaseOrderDto>>;
