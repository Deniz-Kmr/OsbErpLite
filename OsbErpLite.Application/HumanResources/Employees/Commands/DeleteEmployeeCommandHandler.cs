using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.HumanResources.Employees.Commands;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _repository;

    public DeleteEmployeeCommandHandler(IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Employee with Id '{request.Id}' was not found.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
