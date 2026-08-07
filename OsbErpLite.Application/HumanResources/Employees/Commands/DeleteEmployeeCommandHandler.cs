using MediatR;
using OsbErpLite.Application.Common.Interfaces;

namespace OsbErpLite.Application.HumanResources.Employees.Commands;

public class DeleteEmployeeCommandHandler : IRequestHandler<DeleteEmployeeCommand, Unit>
{
    private readonly IEmployeeRepository _repository;
    private readonly ILeaveRequestRepository _leaveRequestRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public DeleteEmployeeCommandHandler(
        IEmployeeRepository repository,
        ILeaveRequestRepository leaveRequestRepository,
        IDepartmentRepository departmentRepository)
    {
        _repository = repository;
        _leaveRequestRepository = leaveRequestRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<Unit> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Employee with Id '{request.Id}' was not found.");

        var activeSubordinateCount = await _repository.CountActiveByManagerIdAsync(request.Id, cancellationToken);
        if (activeSubordinateCount > 0)
            throw new InvalidOperationException($"Once {activeSubordinateCount} alt calisanin yoneticisini degistirin.");

        var pendingLeaveCount = await _leaveRequestRepository.CountPendingByEmployeeIdAsync(request.Id, cancellationToken);
        if (pendingLeaveCount > 0)
            throw new InvalidOperationException("Bekleyen izin talebi olan calisan silinemez.");

        var isManager = await _departmentRepository.IsManagerOfAnyDepartmentAsync(request.Id, cancellationToken);
        if (isManager)
            throw new InvalidOperationException("Bu calisan bir departmanin yoneticisi, once yeni yonetici atayin.");

        entity.IsActive = false;
        await _repository.UpdateAsync(entity, cancellationToken);
        return Unit.Value;
    }
}
