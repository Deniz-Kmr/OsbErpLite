using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Common.Models;
using OsbErpLite.Application.HumanResources.Positions.Dtos;

namespace OsbErpLite.Application.HumanResources.Positions.Queries;

public class GetAllPositionsQueryHandler : IRequestHandler<GetAllPositionsQuery, PaginatedList<PositionDto>>
{
    private readonly IPositionRepository _repository;
    private readonly IMapper _mapper;

    public GetAllPositionsQueryHandler(IPositionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PaginatedList<PositionDto>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _repository.GetAllAsync(request.Page, request.PageSize, cancellationToken);
        var dtos = _mapper.Map<List<PositionDto>>(items);
        return new PaginatedList<PositionDto>(dtos, totalCount, request.Page, request.PageSize);
    }
}
