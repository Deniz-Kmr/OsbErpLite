using MediatR;
using OsbErpLite.Application.SalesOrders.Dtos;

namespace OsbErpLite.Application.SalesOrders.Commands;

public record RejectSalesOrderCommand(Guid Id, Guid RejectedByEmployeeId) : IRequest<SalesOrderDto>;
