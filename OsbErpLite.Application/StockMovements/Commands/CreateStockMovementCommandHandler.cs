using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.StockMovements.Dtos;
using OsbErpLite.Domain.Entities;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.StockMovements.Commands;

public class CreateStockMovementCommandHandler : IRequestHandler<CreateStockMovementCommand, StockMovementDto>
{
    private readonly IStockMovementRepository _stockMovementRepository;
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public CreateStockMovementCommandHandler(
        IStockMovementRepository stockMovementRepository,
        IProductRepository productRepository,
        IMapper mapper)
    {
        _stockMovementRepository = stockMovementRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<StockMovementDto> Handle(CreateStockMovementCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with Id '{request.ProductId}' was not found.");

        if (request.MovementType == StockMovementType.Out)
        {
            var newQuantity = product.StockQuantity - request.Quantity;
            if (newQuantity < 0)
                throw new InvalidOperationException(
                    $"Yetersiz stok. Mevcut: {product.StockQuantity}, talep edilen cikis: {request.Quantity}.");
        }

        var entity = _mapper.Map<StockMovement>(request);
        entity.Date = DateTime.UtcNow;

        if (request.MovementType == StockMovementType.In)
            product.StockQuantity += request.Quantity;
        else
            product.StockQuantity -= request.Quantity;

        await _productRepository.UpdateAsync(product, cancellationToken);
        var created = await _stockMovementRepository.AddAsync(entity, cancellationToken);

        return _mapper.Map<StockMovementDto>(created);
    }
}
