using MediatR;
using OsbErpLite.Application.PurchaseOrders.Dtos;

namespace OsbErpLite.Application.PurchaseOrders.Commands;

public record ApprovePurchaseOrderCommand(Guid Id, Guid ApprovedByEmployeeId) : IRequest<PurchaseOrderDto>;
