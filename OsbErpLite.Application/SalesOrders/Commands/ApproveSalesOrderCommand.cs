using MediatR;
using OsbErpLite.Application.SalesOrders.Dtos;

namespace OsbErpLite.Application.SalesOrders.Commands;

public record ApproveSalesOrderCommand(Guid Id, Guid ApprovedByEmployeeId) : IRequest<SalesOrderDto>;
