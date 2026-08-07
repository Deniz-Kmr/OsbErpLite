using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.BusinessPartners.Commands;

public class DeleteBusinessPartnerCommandHandler : IRequestHandler<DeleteBusinessPartnerCommand, Unit>
{
    private readonly IBusinessPartnerRepository _repository;
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly ISalesOrderRepository _salesOrderRepository;

    public DeleteBusinessPartnerCommandHandler(
        IBusinessPartnerRepository repository,
        IPurchaseOrderRepository purchaseOrderRepository,
        ISalesOrderRepository salesOrderRepository)
    {
        _repository = repository;
        _purchaseOrderRepository = purchaseOrderRepository;
        _salesOrderRepository = salesOrderRepository;
    }

    public async Task<Unit> Handle(DeleteBusinessPartnerCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"BusinessPartner with Id '{request.Id}' was not found.");

        var incompletePurchaseOrders = await _purchaseOrderRepository.CountIncompleteBySupplierIdAsync(request.Id, cancellationToken);
        var incompleteSalesOrders = await _salesOrderRepository.CountIncompleteByCustomerIdAsync(request.Id, cancellationToken);
        var totalIncomplete = incompletePurchaseOrders + incompleteSalesOrders;

        if (totalIncomplete > 0)
            throw new InvalidOperationException($"Bu is ortagina ait {totalIncomplete} tamamlanmamis siparis var, once onlari tamamlayin/iptal edin.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
