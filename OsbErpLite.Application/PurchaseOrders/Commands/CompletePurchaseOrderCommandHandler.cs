using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.PurchaseOrders.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.PurchaseOrders.Commands;

public class CompletePurchaseOrderCommandHandler : IRequestHandler<CompletePurchaseOrderCommand, PurchaseOrderDto>
{
    private readonly IPurchaseOrderRepository _purchaseOrderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IMapper _mapper;

    public CompletePurchaseOrderCommandHandler(
        IPurchaseOrderRepository purchaseOrderRepository,
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IMapper mapper)
    {
        _purchaseOrderRepository = purchaseOrderRepository;
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _mapper = mapper;
    }

    public async Task<PurchaseOrderDto> Handle(CompletePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _purchaseOrderRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"PurchaseOrder with Id '{request.Id}' was not found.");

        if (entity.Status != OrderStatus.Approved)
            throw new InvalidOperationException($"Sadece Approved durumundaki siparisler tamamlanabilir. Mevcut durum: {entity.Status}");

        // her satir icin stok giris hareketi olustur
        foreach (var line in entity.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with Id '{line.ProductId}' was not found.");

            product.StockQuantity += line.Quantity;
            await _productRepository.UpdateAsync(product, cancellationToken);

            var stockMovement = new StockMovement
            {
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                MovementType = StockMovementType.In,
                Date = DateTime.UtcNow,
                Note = $"Satin alma siparisi tamamlandi. PO: {entity.Id}"
            };
            await _stockMovementRepository.AddAsync(stockMovement, cancellationToken);
        }

        entity.Status = OrderStatus.Completed;
        await _purchaseOrderRepository.UpdateAsync(entity, cancellationToken);

        return _mapper.Map<PurchaseOrderDto>(entity);
    }
}
