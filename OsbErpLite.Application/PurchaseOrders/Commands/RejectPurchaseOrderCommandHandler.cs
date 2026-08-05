using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.PurchaseOrders.Commands;

public class RejectPurchaseOrderCommandHandler : IRequestHandler<RejectPurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IMapper _mapper;

    public RejectPurchaseOrderCommandHandler(IPurchaseOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PurchaseOrderDto> Handle(RejectPurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"PurchaseOrder with Id '{request.Id}' was not found.");

        if (entity.Status != OrderStatus.Draft && entity.Status != OrderStatus.Pending)
            throw new InvalidOperationException($"Sadece Draft veya Pending durumundaki siparisler reddedilebilir. Mevcut durum: {entity.Status}");

        entity.Status = OrderStatus.Rejected;

        await _repository.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<PurchaseOrderDto>(entity);
    }
}
