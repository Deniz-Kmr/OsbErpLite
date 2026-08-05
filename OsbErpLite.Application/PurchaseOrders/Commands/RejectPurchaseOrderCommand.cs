using MediatR;
using OsbErpLite.Application.PurchaseOrders.Dtos;

namespace OsbErpLite.Application.PurchaseOrders.Commands;

public record RejectPurchaseOrderCommand(Guid Id, Guid RejectedByEmployeeId) : IRequest<PurchaseOrderDto>;
