using MediatR;
using OsbErpLite.Application.SalesOrders.Dtos;

namespace OsbErpLite.Application.SalesOrders.Queries;

public record GetSalesOrderByIdQuery(Guid Id) : IRequest<SalesOrderDto?>;
