using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.HumanResources.Positions.Commands;

public class DeletePositionCommandHandler : IRequestHandler<DeletePositionCommand, Unit>
{
    private readonly IPositionRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;

    public DeletePositionCommandHandler(IPositionRepository repository, IEmployeeRepository employeeRepository)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
    }

    public async Task<Unit> Handle(DeletePositionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Position with Id '{request.Id}' was not found.");

        var activeEmployeeCount = await _employeeRepository.CountActiveByPositionIdAsync(request.Id, cancellationToken);
        if (activeEmployeeCount > 0)
            throw new InvalidOperationException($"Bu pozisyonda {activeEmployeeCount} aktif calisan var, once onlari baska pozisyona tasiyin.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
