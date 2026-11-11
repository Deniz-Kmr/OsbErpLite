using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.Warehouses.Commands;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Unit>
{
    private readonly IWarehouseRepository _repository;
    private readonly IProductRepository _productRepository;

    public DeleteWarehouseCommandHandler(IWarehouseRepository repository, IProductRepository productRepository)
    {
        _repository = repository;
        _productRepository = productRepository;
    }

    public async Task<Unit> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Warehouse with Id '{request.Id}' was not found.");

        var activeProductCount = await _productRepository.CountActiveByWarehouseIdAsync(request.Id, cancellationToken);
        if (activeProductCount > 0)
            throw new InvalidOperationException($"Bu depoda {activeProductCount} aktif urun var, once onlari baska depoya tasiyin.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
