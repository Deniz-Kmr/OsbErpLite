using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.HumanResources.Positions.Commands;

public class DeletePositionCommandHandler : IRequestHandler<DeletePositionCommand, Unit>
{
    private readonly IPositionRepository _repository;

    public DeletePositionCommandHandler(IPositionRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Position with Id '{request.Id}' was not found.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
