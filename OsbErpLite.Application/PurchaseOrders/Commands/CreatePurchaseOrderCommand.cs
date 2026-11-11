using MediatR;
using OsbErpLite.Application.PurchaseOrders.Dtos;

namespace OsbErpLite.Application.PurchaseOrders.Commands;

public record CreatePurchaseOrderCommand(
    Guid SupplierId,
    DateTime OrderDate,
    List<CreatePurchaseOrderLineDto> Lines
) : IRequest<PurchaseOrderDto>;
