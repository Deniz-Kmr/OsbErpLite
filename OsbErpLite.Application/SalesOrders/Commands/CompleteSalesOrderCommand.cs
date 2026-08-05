using MediatR;
using OsbErpLite.Application.SalesOrders.Dtos;

namespace OsbErpLite.Application.SalesOrders.Commands;

public record CompleteSalesOrderCommand(Guid Id, Guid CompletedByEmployeeId) : IRequest<SalesOrderDto>;
