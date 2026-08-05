using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.Positions.Dtos;

namespace OsbErpLite.Application.HumanResources.Positions.Queries;

public class GetPositionByIdQueryHandler : IRequestHandler<GetPositionByIdQuery, PositionDto?>
{
    private readonly IPositionRepository _repository;
    private readonly IMapper _mapper;

    public GetPositionByIdQueryHandler(IPositionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PositionDto?> Handle(GetPositionByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<PositionDto>(entity);
    }
}
