using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;
using OsbErpLite.Domain.Enums;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Commands;

public class RejectLeaveRequestCommandHandler : IRequestHandler<RejectLeaveRequestCommand, LeaveRequestDto>
{
    private readonly ILeaveRequestRepository _repository;
    private readonly IMapper _mapper;

    public RejectLeaveRequestCommandHandler(ILeaveRequestRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<LeaveRequestDto> Handle(RejectLeaveRequestCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"LeaveRequest with Id '{request.Id}' was not found.");

        if (entity.Status != LeaveRequestStatus.Pending)
            throw new InvalidOperationException("Sadece Pending durumundaki izin talepleri reddedilebilir.");

        entity.Status = LeaveRequestStatus.Rejected;
        entity.ApprovedByEmployeeId = request.RejectedByEmployeeId;

        await _repository.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<LeaveRequestDto>(entity);
    }
}
