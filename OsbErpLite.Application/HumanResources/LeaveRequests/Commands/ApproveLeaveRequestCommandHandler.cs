using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Commands;

public class ApproveLeaveRequestCommandHandler : IRequestHandler<ApproveLeaveRequestCommand, LeaveRequestDto>
{
    private readonly ILeaveRequestRepository _repository;
    private readonly IMapper _mapper;

    public ApproveLeaveRequestCommandHandler(ILeaveRequestRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<LeaveRequestDto> Handle(ApproveLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"LeaveRequest with Id '{request.Id}' was not found.");

        if (entity.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException("Sadece Pending durumundaki izin talepleri onaylanabilir.");

        entity.Status = LeaveRequestStatus.Approved;
        entity.ApprovedByEmployeeId = request.ApprovedByEmployeeId;

        await _repository.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<LeaveRequestDto>(entity);
    }
}
