using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Warehouses.Dtos;
using OsbErpLite.Domain.Entities;

namespace OsbErpLite.Application.Warehouses.Commands;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, WarehouseDto>
{
    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;

    public CreateWarehouseCommandHandler(IWarehouseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<WarehouseDto> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<Warehouse>(request);
        var created = await _repository.AddAsync(entity, cancellationToken);
        return _mapper.Map<WarehouseDto>(created);
    }
}
