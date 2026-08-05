using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Queries;

public class GetLeaveRequestByIdQueryHandler : IRequestHandler<GetLeaveRequestByIdQuery, LeaveRequestDto?>
{
    private readonly ILeaveRequestRepository _repository;
    private readonly IMapper _mapper;

    public GetLeaveRequestByIdQueryHandler(ILeaveRequestRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<LeaveRequestDto?> Handle(GetLeaveRequestByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<LeaveRequestDto>(entity);
    }
}
