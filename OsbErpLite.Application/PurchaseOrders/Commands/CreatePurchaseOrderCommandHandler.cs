using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.PurchaseOrders.Commands;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _repository;
    private readonly IMapper _mapper;

    public CreatePurchaseOrderCommandHandler(IPurchaseOrderRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PurchaseOrderDto> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = new PurchaseOrder
        {
            SupplierId = request.SupplierId,
            OrderDate = request.OrderDate,
            Status = OrderStatus.Draft,
            Lines = request.Lines.Select(l => new PurchaseOrderLine
            {
                ProductId = l.ProductId,
                Quantity = l.Quantity,
                UnitPrice = l.UnitPrice
            }).ToList()
        };

        entity.TotalAmount = entity.Lines.Sum(l => l.Quantity * l.UnitPrice);

        var created = await _repository.AddAsync(entity, cancellationToken);
        return _mapper.Map<PurchaseOrderDto>(created);
    }
}
