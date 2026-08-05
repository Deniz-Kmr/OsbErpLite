using AutoMapper;
using MediatR;
using OsbErpLite.Application.Common.Interfaces;
using OsbErpLite.Application.Warehouses.Dtos;

namespace OsbErpLite.Application.Warehouses.Queries;

public class GetWarehouseByIdQueryHandler : IRequestHandler<GetWarehouseByIdQuery, WarehouseDto?>
{
    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;

    public GetWarehouseByIdQueryHandler(IWarehouseRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<WarehouseDto?> Handle(GetWarehouseByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.Id, cancellationToken);
        return entity is null ? null : _mapper.Map<WarehouseDto>(entity);
    }
}
