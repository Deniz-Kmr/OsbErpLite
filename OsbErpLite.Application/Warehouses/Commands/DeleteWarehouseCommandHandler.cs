using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.Warehouses.Commands;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Unit>
{
    private readonly IWarehouseRepository _repository;

    public DeleteWarehouseCommandHandler(IWarehouseRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Warehouse with Id '{request.Id}' was not found.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
