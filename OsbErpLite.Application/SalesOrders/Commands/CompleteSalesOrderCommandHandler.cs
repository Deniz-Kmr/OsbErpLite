using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.SalesOrders.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.SalesOrders.Commands;

public class CompleteSalesOrderCommandHandler : IRequestHandler<CompleteSalesOrderCommand, SalesOrderDto>
{
    private readonly ISalesOrderRepository _salesOrderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IMapper _mapper;

    public CompleteSalesOrderCommandHandler(
        ISalesOrderRepository salesOrderRepository,
        IProductRepository productRepository,
        IStockMovementRepository stockMovementRepository,
        IMapper mapper)
    {
        _salesOrderRepository = salesOrderRepository;
        _productRepository = productRepository;
        _stockMovementRepository = stockMovementRepository;
        _mapper = mapper;
    }

    public async Task<SalesOrderDto> Handle(CompleteSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var entity = await _salesOrderRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"SalesOrder with Id '{request.Id}' was not found.");

        if (entity.Status != OrderStatus.Approved)
            throw new InvalidOperationException($"Sadece Approved durumundaki siparisler tamamlanabilir. Mevcut durum: {entity.Status}");

        // stok yeterliligi kontrolu
        foreach (var line in entity.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with Id '{line.ProductId}' was not found.");

            if (product.StockQuantity < line.Quantity)
                throw new InvalidOperationException(
                    $"Yetersiz stok. Urun: {product.Name} (SKU: {product.SKU}), Mevcut: {product.StockQuantity}, Talep: {line.Quantity}");
        }

        // stok cikis hareketleri olustur
        foreach (var line in entity.Lines)
        {
            var product = await _productRepository.GetByIdAsync(line.ProductId, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with Id '{line.ProductId}' was not found.");

            product.StockQuantity -= line.Quantity;
            await _productRepository.UpdateAsync(product, cancellationToken);

            var stockMovement = new StockMovement
            {
                ProductId = line.ProductId,
                Quantity = line.Quantity,
                MovementType = StockMovementType.Out,
                Date = DateTime.UtcNow,
                Note = $"Satis siparisi tamamlandi. SO: {entity.Id}"
            };
            await _stockMovementRepository.AddAsync(stockMovement, cancellationToken);
        }

        entity.Status = OrderStatus.Completed;
        await _salesOrderRepository.UpdateAsync(entity, cancellationToken);

        return _mapper.Map<SalesOrderDto>(entity);
    }
}
