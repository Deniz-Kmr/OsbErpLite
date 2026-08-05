using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.LeaveRequests.Dtos;

namespace OsbErpLite.Application.HumanResources.LeaveRequests.Queries;

public class GetAllLeaveRequestsQueryHandler : IRequestHandler<GetAllLeaveRequestsQuery, PaginatedList<LeaveRequestDto>>
{
    private readonly ILeaveRequestRepository _repository;
    private readonly IMapper _mapper;

    public GetAllLeaveRequestsQueryHandler(ILeaveRequestRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<LeaveRequestDto>> Handle(GetAllLeaveRequestsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(
            request.Page, request.PageSize, request.EmployeeId, request.Status, cancellationToken);
        var dtos = _mapper.Map<List<LeaveRequestDto>>(items);
        return new PaginatedList<LeaveRequestDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
