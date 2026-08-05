using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.HumanResources.Departments.Commands;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _repository;

    public DeleteDepartmentCommandHandler(IDepartmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Department with Id '{request.Id}' was not found.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
