using MediatR;
using OsbErpLite.Application.SalesOrders.Dtos;

namespace OsbErpLite.Application.SalesOrders.Commands;

public record CreateSalesOrderCommand(
    Guid CustomerId,
    DateTime OrderDate,
    List<CreateSalesOrderLineDto> Lines
) : IRequest<SalesOrderDto>;
