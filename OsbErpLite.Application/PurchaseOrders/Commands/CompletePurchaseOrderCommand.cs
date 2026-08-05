using MediatR;
using OsbErpLite.Application.PurchaseOrders.Dtos;

namespace OsbErpLite.Application.PurchaseOrders.Commands;

public record CompletePurchaseOrderCommand(Guid Id, Guid CompletedByEmployeeId) : IRequest<PurchaseOrderDto>;
