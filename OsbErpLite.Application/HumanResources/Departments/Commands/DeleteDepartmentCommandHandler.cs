using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.HumanResources.Departments.Commands;

public class DeleteDepartmentCommandHandler : IRequestHandler<DeleteDepartmentCommand, Unit>
{
    private readonly IDepartmentRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;

    public DeleteDepartmentCommandHandler(IDepartmentRepository repository, IEmployeeRepository employeeRepository)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
    }

    public async Task<Unit> Handle(DeleteDepartmentCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Department with Id '{request.Id}' was not found.");

        var activeEmployeeCount = await _employeeRepository.CountActiveByDepartmentIdAsync(request.Id, cancellationToken);
        if (activeEmployeeCount > 0)
            throw new InvalidOperationException($"Bu departmanda {activeEmployeeCount} aktif calisan var, once onlari baska departmana tasiyin.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
