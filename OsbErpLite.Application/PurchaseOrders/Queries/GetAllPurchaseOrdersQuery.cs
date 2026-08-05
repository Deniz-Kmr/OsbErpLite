using MediatR;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.PurchaseOrders.Queries;

public record GetAllPurchaseOrdersQuery(int Page = 1, int PageSize = 10, OrderStatus? Status = null)
    : IRequest<PaginatedList<PurchaseOrderDto>>;
