using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.Products.Commands;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Unit>
{
    private readonly IProductRepository _repository;

    public DeleteProductCommandHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Product with Id '{request.Id}' was not found.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
