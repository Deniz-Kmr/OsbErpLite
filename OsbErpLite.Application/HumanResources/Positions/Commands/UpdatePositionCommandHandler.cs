using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.HumanResources.Positions.Dtos;

namespace OsbErpLite.Application.HumanResources.Positions.Commands;

public class UpdatePositionCommandHandler : IRequestHandler<UpdatePositionCommand, PositionDto>
{
    private readonly IPositionRepository _repository;
    private readonly IMapper _mapper;

    public UpdatePositionCommandHandler(IPositionRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<PositionDto> Handle(UpdatePositionCommand request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"Position with Id '{request.Id}' was not found.");

        _mapper.Map(request, entity);
        await _repository.UpdateAsync(entity, cancellationToken);
        return _mapper.Map<PositionDto>(entity);
    }
}
